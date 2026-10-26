using Newtonsoft.Json;
using Revmax_Interface_Promun.Processing;
using Revmax_Interface_Promun.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Revmax_Interface_Promun
{
    internal class Convolve
    {
        private StringBuilder m_Sb;
        private bool m_bDirty;
        private System.IO.FileSystemWatcher m_Watcher;
        private bool m_bIsWatching;

        ArrayList HH = new ArrayList();
        ArrayList ItemCode = new ArrayList();
        ArrayList ItemName1 = new ArrayList();
        ArrayList ItemName2 = new ArrayList();
        ArrayList Price = new ArrayList();
        ArrayList Quantity = new ArrayList();
        ArrayList Tax = new ArrayList();
        ArrayList TaxR = new ArrayList();
        ArrayList Amount = new ArrayList();

        String PromoFile = @".\Message.txt";
        String PromoMessage = " ";

        String RATE = "", SIGNATURE = "", INVOICEBASECURRENCY = "",
            BRANCHNAME = "", INVOICENUMBER = "", INVOICEDATE = "",
            CUSTOMERNUM = "", CUSTOMERBPN = "", CUSTOMERNAME = "",
            CUSTOMERVATNUMBER = "", CUSTOMERADDRESS = "",
            CUSTOMERTELEPHONENUMBER = "", PAYTERMS = "",
            CUSTOMERPAYER = "", INVOICEAMOUNT = "",
            INVOICETAXAMOUNT = "", INVOICECOMMENT = "",
            PRINTER = "", CASHIER = "", StraightPrint = "",
            errorCount = "", invoiceCount = "";

        // ------------------------------------------------------------------
        // Safe number helpers (culture independent: '.' is always the decimal point)
        // ------------------------------------------------------------------
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // Removes currency symbols, thousands separators and spaces from DATA values
        private static string CleanNumber(string s)
        {
            return (s ?? "").Replace("$", "").Replace(",", "").Replace(" ", "").Trim();
        }

        private static bool TryDec(string s, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(s)) return false;
            return decimal.TryParse(s.Trim(), NumberStyles.Number, Inv, out value);
        }

        // Parses or throws a FormatException that tells you WHICH value was bad
        private static decimal Dec(string s)
        {
            if (TryDec(s, out decimal v)) return v;
            throw new FormatException("Cannot parse '" + (s ?? "<null>") + "' as a number.");
        }

        // Detects a detail line like "   1.000 X 1000.00 0.00   1000.00"  (qty X price tax amount)
        private static bool IsDetailLine(string l)
        {
            if (string.IsNullOrWhiteSpace(l)) return false;
            string[] t = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return t.Length >= 4 && TryDec(t[0], out _) && t[1].Equals("X", StringComparison.OrdinalIgnoreCase);
        }

        private static void LogSkipped(string reason, string line)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SkippedLines.log");
                File.AppendAllText(path,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + reason + " | LINE: " + line + Environment.NewLine);
            }
            catch { /* never let logging crash the app */ }
        }

        private string ReduceToEightChars(string input)
        {
            string paddedString = input.PadRight(8);
            return paddedString.Substring(0, 8);
        }

        public bool CountDots(int numberOfDots, string line)
        {
            int counter = 0;
            foreach (char c in line)
            {
                if (c == '.')
                    counter++;
            }
            return counter >= numberOfDots;
        }

        // Resolves product code, description and isProduct flag using cached site products.
        private static void ResolveProduct(
            string rawName, string rawCode,
            out string resolvedCode, out string resolvedDescription, out bool resolvedIsProduct)
        {
            resolvedCode = rawCode;
            resolvedDescription = rawName;
            resolvedIsProduct = false;

            if (ConfigurationManager.AppSettings.Get("GetConfigProductCode") != "true")
            {
                LogMatch(rawName, "SKIPPED: GetConfigProductCode is not 'true' in config");
                return;
            }
            if (RevMaxInterfaceWizard.CachedSiteProducts == null || RevMaxInterfaceWizard.CachedSiteProducts.Count == 0)
            {
                LogMatch(rawName, "SKIPPED: product cache is empty (products.json missing or fetch from MRA failed)");
                return;
            }

            var matched = ProductMatcher.FindBestMatchingProduct(rawName, RevMaxInterfaceWizard.CachedSiteProducts);
            if (matched == null)
            {
                LogMatch(rawName, "NO MATCH among " + RevMaxInterfaceWizard.CachedSiteProducts.Count + " cached products");
                return;
            }

            resolvedCode = matched.ProductCode ?? rawCode;
            resolvedDescription = !string.IsNullOrWhiteSpace(matched.Description) ? matched.Description : rawName;
            resolvedIsProduct = matched.IsProduct ?? false;
            LogMatch(rawName, "MATCHED -> code='" + resolvedCode + "' description='" + resolvedDescription + "' isProduct=" + resolvedIsProduct);
        }

        private static void LogMatch(string rawName, string result)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductMatch.log");
                File.AppendAllText(path,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | '" + rawName + "' | " + result + Environment.NewLine);
            }
            catch { /* never let logging crash the app */ }
        }

        private static string CleanProductName(string product)
        {
            return product.Replace("!", " ").Replace("@", "").Replace("#", "").Replace("$", "")
                .Replace("%", "").Replace("^", "").Replace("&", "").Replace("*", "")
                .Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "")
                .Replace("~", "").Replace("<", "").Replace(">", "").Replace("?", "")
                .Replace("/", "").Replace(@"\", "").Replace("[", "").Replace("]", "")
                .Replace("{", "").Replace("}", "").Replace("|", "").Replace("`", "").Trim();
        }

        public Invoice ReadInvoice(string filepath, string vatFlag, List<ReadCurrencies> CurrenciesList)
        {
            string branchName = "";
            string customerName = "";
            string customerVATNumber = "";
            string invoiceComment = "";
            string product = "";
            string CustomerTel = "";
            string customerEmail = "";
            string customerAddress = "";

            int cntinv = 0;
            int productstartline = 0;
            int productendline = 0;
            int cntr = 0;
            string quantity = "0";
            string amount = "0";
            string price = "0";
            string code = "";
            int i = 0;
            int customerAddStart = 0;
            int CustomerAddEnd = 0;
            string customerTIN = "";

            string invoiceamount = "";
            string cashier = "";
            string Discount = "";
            string InvoiceNumber = "";
            string currentCurrency = "";
            string tax = "";
            string VAT = "";
            decimal overalAmount = 0;
            decimal overalTax = 0;
            string[] stringsToReplace = { ",", "x", "$", ":", "US", "US$", "D", "ZWL", "ZWD" };
            StringBuilder myItemsXMLStringBuilder = new StringBuilder();
            myItemsXMLStringBuilder.Append("<ITEMS>");
            StringBuilder myCurrenciesXMLStringBuilder = new StringBuilder();
            List<item> items = new List<item>();
            List<MraReceiptLine> mraReceiptLines = new List<MraReceiptLine>();

            int itemDotCounter = int.Parse(ConfigurationManager.AppSettings.Get("ItemDotCounter") ?? "1");

            // First pass: find product start/end lines, currency, and invoice flags
            foreach (string kline in File.ReadAllLines(filepath))
            {
                cntinv += 1;
                if (kline.Contains(ConfigurationManager.AppSettings.Get("ProductStartLine")))
                    productstartline = cntinv;

                if (kline.Contains(ConfigurationManager.AppSettings.Get("ProductEndLine")))
                    productendline = cntinv;

                for (int a = 0; a < CurrenciesList.Count; a++)
                {
                    if (kline.Contains(CurrenciesList[a].Keyword))
                        currentCurrency = CurrenciesList[a].Name;
                }

                if (kline.Contains(ConfigurationManager.AppSettings.Get("CustomerName")))
                    customerAddStart = cntinv + 1;

                if (kline.Contains("PHONE NO:"))
                    CustomerAddEnd = cntinv;

                if (currentCurrency == "")
                    currentCurrency = ConfigurationManager.AppSettings.Get("DefaultCurrency");

                if (kline.ToLower().Contains("credit note"))
                    addToConfig("InvoiceFlag", "02");
                else if (kline.ToLower().Contains("tax invoice"))
                    addToConfig("InvoiceFlag", "01");
            }

            string[] allLines = File.ReadAllLines(filepath);
            string pendingName = "";
            string pendingQty = "";

            // Builds one item from a name-first layout detail line: "<qty> X <price> <tax> <amount>"
            bool AddNameFirstItem(string nName, string nDetailLine, string nNameQty)
            {
                string[] nParts = nDetailLine
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !x.Equals("X", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (nParts.Length < 3)
                {
                    LogSkipped("Too few columns in detail line", nDetailLine);
                    return false;
                }

                string nQty = CleanNumber(nParts[0]);
                string nPrice = CleanNumber(nParts[1]);
                string nAmt = CleanNumber(nParts[nParts.Length - 1]);
                string nTaxCol = nParts.Length >= 4 ? CleanNumber(nParts[nParts.Length - 2]) : "0";

                if (!TryDec(nQty, out decimal nQtyVal) ||
                    !TryDec(nPrice, out decimal nPriceVal) ||
                    !TryDec(nAmt, out decimal nAmtVal))
                {
                    LogSkipped("Invalid qty/price/amount. qty='" + nQty + "' price='" + nPrice + "' amount='" + nAmt + "'", nDetailLine);
                    return false;
                }
                // Quantity printed on the name line (e.g. "Dacof Dry   2.5") wins over the "1.000 X" figure.
                // Unit price is then derived from the line amount so qty * price = amount.
                if (TryDec(nNameQty, out decimal nNameQtyVal) && nNameQtyVal > 0)
                {
                    nQtyVal = nNameQtyVal;
                    nQty = nNameQtyVal.ToString(Inv);
                    nPriceVal = Math.Round(nAmtVal / nNameQtyVal, 2);
                    nPrice = nPriceVal.ToString(Inv);
                }

                TryDec(nTaxCol, out decimal nTaxColVal);

                // Tax column > 0 means standard-rated (A), otherwise zero-rated/exempt (E)
                string nVat = nTaxColVal > 0
                    ? ConfigurationManager.AppSettings.Get("VatA")
                    : ConfigurationManager.AppSettings.Get("VatE");
                decimal nRate = Dec(nVat);

                decimal nTx;
                if (ConfigurationManager.AppSettings.Get("VatFlag") == "0")
                    nTx = nAmtVal * nRate;
                else
                    nTx = Math.Round(nAmtVal - nAmtVal / (1 + nRate), 2);
                string nTxStr = nTx.ToString(Inv);

                string nProduct = CleanProductName(nName);

                i += 1;
                code = ".";
                overalTax += nTx;
                overalAmount += nAmtVal;

                items.Add(new item
                {
                    HH = (i + 1).ToString(),
                    ITEMCODE = code,
                    ITEMNAME1 = nProduct,
                    ITEMNAME2 = nProduct,
                    Quantity = nQty,
                    Price = nPrice,
                    Amount = nAmt,
                    Tax = nTxStr,
                    TaxR = nVat
                });

                ResolveProduct(nProduct, code,
                    out string nResolvedCode, out string nResolvedDescription, out bool nResolvedIsProduct);

                mraReceiptLines.Add(new MraReceiptLine
                {
                    ProductCode = nResolvedCode,
                    Description = nResolvedDescription,
                    UnitPrice = nPriceVal,
                    Quantity = nQtyVal,
                    Discount = 0m,
                    Total = nAmtVal - nTx,
                    TotalVAT = nTx,
                    TaxRateId = nRate > 0 ? "A" : "E",
                    IsProduct = nResolvedIsProduct
                });

                myItemsXMLStringBuilder.Append("<ITEM>");
                myItemsXMLStringBuilder.Append("<HH>" + (i + 1).ToString() + "</HH>");
                myItemsXMLStringBuilder.Append("<ITEMCODE>" + code + "</ITEMCODE>");
                myItemsXMLStringBuilder.Append("<ITEMNAME1>" + nProduct + "</ITEMNAME1>");
                myItemsXMLStringBuilder.Append("<ITEMNAME2>" + nProduct + "</ITEMNAME2>");
                myItemsXMLStringBuilder.Append("<QTY>" + nQty + "</QTY>");
                myItemsXMLStringBuilder.Append("<PRICE>" + nPrice + "</PRICE>");
                myItemsXMLStringBuilder.Append("<AMT>" + nAmt + "</AMT>");
                myItemsXMLStringBuilder.Append("<TAX>" + nTxStr + "</TAX>");
                myItemsXMLStringBuilder.Append("<TAXR>" + nVat + "</TAXR>");
                myItemsXMLStringBuilder.Append("</ITEM>");
                return true;
            }

            // Second pass: parse line items and customer details
            foreach (string line in allLines)
            {
                cntr += 1;

                if (ConfigurationManager.AppSettings.Get("MultiLineProduct") == "1")
                {
                    bool inProductArea = cntr > productstartline && cntr < productendline;
                    bool hasDots = CountDots(itemDotCounter, line);
                    bool firstTokenIsNumber = decimal.TryParse(line.Split(' ')[0], NumberStyles.Number, Inv, out _);

                    if (inProductArea && IsDetailLine(line))
                    {
                        // Name-first layout, detail line: "<qty> X <price> <tax> <amount>"
                        string nm = string.IsNullOrWhiteSpace(pendingName) ? "Item" : pendingName;
                        AddNameFirstItem(nm, line, pendingQty);
                        pendingName = "";
                        pendingQty = "";
                    }
                    else if (inProductArea && !string.IsNullOrWhiteSpace(line) && !line.Contains("----")
                        && cntr < allLines.Length && IsDetailLine(allLines[cntr]))
                    {
                        // Name-first layout, name line: the NEXT line is the detail line
                        string[] pn = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (pn.Length > 1 && TryDec(pn[pn.Length - 1], out _))
                        {
                            pendingQty = CleanNumber(pn[pn.Length - 1]);
                            pendingName = string.Join(" ", pn.Take(pn.Length - 1));
                        }
                        else
                        {
                            pendingQty = "";
                            pendingName = line.Trim();
                        }
                    }
                    else if (inProductArea && hasDots && firstTokenIsNumber)
                    {
                        // Line 1 of an item: "<qty> ... <price>"  -> we only need the price here
                        string[] arrayprod = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrayprod.Length > 0)
                        {
                            i += 1;
                            code = ".";
                            price = arrayprod[arrayprod.Length - 1];
                            price = stringsToReplace.Aggregate(price, (current, str) => current.Replace(str, "")).Trim();
                        }
                    }
                    else if (inProductArea && hasDots && !firstTokenIsNumber
                        && !line.Contains("Price")
                        && !line.Contains("----------")
                        && !line.Equals(""))
                    {
                        // Line 2 of an item: "<description...> <amount> <taxflag>"
                        string[] arrayprod = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        code = ".";

                        if (arrayprod.Length < 3)
                        {
                            LogSkipped("Too few columns", line);
                            product = "";
                        }
                        else
                        {
                            for (int m = 0; m < arrayprod.Length - 2; m++)
                            {
                                product += " " + arrayprod[m];
                            }
                            amount = CleanNumber(arrayprod[arrayprod.Length - 2]);

                            if (!TryDec(amount, out decimal amountVal) ||
                                !TryDec(price, out decimal priceVal) ||
                                priceVal == 0m)
                            {
                                // THIS was the line that crashed. Now we log the bad values and skip the line.
                                LogSkipped("Invalid amount/price. amount='" + amount + "' price='" + price + "'", line);
                                product = "";
                            }
                            else
                            {
                                quantity = (amountVal / priceVal).ToString("0.000", Inv);

                                if (!arrayprod[arrayprod.Length - 1].Contains("NV"))
                                {
                                    VAT = ConfigurationManager.AppSettings.Get("VatA");
                                    tax = VAT;
                                }
                                else
                                {
                                    VAT = ConfigurationManager.AppSettings.Get("VatE");
                                    tax = VAT;
                                }

                                decimal taxRate = Dec(tax);
                                string taxamount = (amountVal * taxRate).ToString(Inv);

                                string tx = "";
                                if (ConfigurationManager.AppSettings.Get("VatFlag") == "1")
                                    tx = Math.Round(amountVal - amountVal / (1 + taxRate), 2).ToString(Inv);
                                else if (ConfigurationManager.AppSettings.Get("VatFlag") == "0")
                                    tx = taxamount;

                                product = CleanProductName(product);

                                decimal txVal = Dec(tx);
                                overalTax += txVal;

                                items.Add(new item
                                {
                                    HH = (i + 1).ToString(),
                                    ITEMCODE = code,
                                    ITEMNAME1 = product,
                                    ITEMNAME2 = product,
                                    Quantity = quantity,
                                    Price = price,
                                    Amount = amount,
                                    Tax = tx,
                                    TaxR = VAT
                                });

                                overalAmount += amountVal;

                                // Product matching for MRA submission
                                ResolveProduct(product, code,
                                    out string resolvedCode1, out string resolvedDescription1, out bool resolvedIsProduct1);

                                mraReceiptLines.Add(new MraReceiptLine
                                {
                                    ProductCode = resolvedCode1,
                                    Description = resolvedDescription1,
                                    UnitPrice = priceVal,
                                    Quantity = Dec(quantity),
                                    Discount = 0m,
                                    Total = amountVal - txVal,
                                    TotalVAT = txVal,
                                    TaxRateId = Dec(VAT) > 0 ? "A" : "E",
                                    IsProduct = resolvedIsProduct1
                                });

                                myItemsXMLStringBuilder.Append("<ITEM>");
                                myItemsXMLStringBuilder.Append("<HH>" + (i + 1).ToString() + "</HH>");
                                myItemsXMLStringBuilder.Append("<ITEMCODE>" + code + "</ITEMCODE>");
                                myItemsXMLStringBuilder.Append("<ITEMNAME1>" + product + "</ITEMNAME1>");
                                myItemsXMLStringBuilder.Append("<ITEMNAME2>" + product + "</ITEMNAME2>");
                                myItemsXMLStringBuilder.Append("<QTY>" + quantity + "</QTY>");
                                myItemsXMLStringBuilder.Append("<PRICE>" + price + "</PRICE>");
                                myItemsXMLStringBuilder.Append("<AMT>" + amount + "</AMT>");
                                myItemsXMLStringBuilder.Append("<TAX>" + tx + "</TAX>");
                                myItemsXMLStringBuilder.Append("<TAXR>" + VAT + "</TAXR>");
                                myItemsXMLStringBuilder.Append("</ITEM>");

                                product = "";
                            }
                        }
                    }
                }
                else
                {
                    if ((cntr > productstartline && cntr < productendline)
                        && CountDots(itemDotCounter, line)
                        && !(line.Contains("Qty.") || line.Contains("Price.") || line.Contains("Amt.")))
                    {
                        string[] arrayprod = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrayprod.Length < 6)
                        {
                            LogSkipped("Too few columns", line);
                        }
                        else
                        {
                            i += 1;
                            code = ".";
                            for (int m = 0; m < arrayprod.Length - 5; m++)
                            {
                                product += " " + arrayprod[m];
                            }

                            quantity = arrayprod[arrayprod.Length - 5];
                            quantity = quantity.Replace("$", "").Replace(",", "").Trim();

                            price = arrayprod[arrayprod.Length - 3];
                            price = stringsToReplace.Aggregate(price, (current, str) => current.Replace(str, "")).Trim();

                            string vatText = arrayprod[arrayprod.Length - 2].Replace("%", "");

                            if (!TryDec(quantity, out decimal qtyVal) ||
                                !TryDec(price, out decimal priceVal) ||
                                !TryDec(vatText, out decimal vatPercent))
                            {
                                LogSkipped("Invalid qty/price/vat. qty='" + quantity + "' price='" + price + "' vat='" + vatText + "'", line);
                                product = "";
                                i -= 1;
                            }
                            else
                            {
                                decimal amountVal = Math.Round(priceVal * Math.Round(qtyVal, 2), 2);
                                amount = amountVal.ToString(Inv);

                                if (vatPercent > 0)
                                {
                                    VAT = ConfigurationManager.AppSettings.Get("VatA");
                                    tax = VAT;
                                }
                                else
                                {
                                    VAT = ConfigurationManager.AppSettings.Get("VatE");
                                    tax = VAT;
                                }

                                decimal taxRate = Dec(tax);
                                string taxamount = (amountVal * taxRate).ToString("0.00", Inv);

                                string tx = "";
                                if (ConfigurationManager.AppSettings.Get("VatFlag") == "1")
                                    tx = Math.Round(amountVal - amountVal / (1 + taxRate), 2).ToString(Inv);
                                else if (ConfigurationManager.AppSettings.Get("VatFlag") == "0")
                                    tx = taxamount;

                                product = CleanProductName(product);

                                decimal txVal = Dec(tx);
                                overalTax += txVal;

                                items.Add(new item
                                {
                                    HH = (i + 1).ToString(),
                                    ITEMCODE = code,
                                    ITEMNAME1 = product,
                                    ITEMNAME2 = product,
                                    Quantity = quantity,
                                    Price = price,
                                    Amount = amount,
                                    Tax = tx,
                                    TaxR = VAT
                                });

                                overalAmount += amountVal;

                                // Product matching for MRA submission
                                ResolveProduct(product, code,
                                    out string resolvedCode2, out string resolvedDescription2, out bool resolvedIsProduct2);

                                mraReceiptLines.Add(new MraReceiptLine
                                {
                                    ProductCode = resolvedCode2,
                                    Description = resolvedDescription2,
                                    UnitPrice = priceVal,
                                    Quantity = qtyVal,
                                    Discount = 0m,
                                    Total = amountVal - txVal,
                                    TotalVAT = txVal,
                                    TaxRateId = Dec(VAT) > 0 ? "A" : "E",
                                    IsProduct = resolvedIsProduct2
                                });

                                myItemsXMLStringBuilder.Append("<ITEM>");
                                myItemsXMLStringBuilder.Append("<HH>" + (i + 1).ToString() + "</HH>");
                                myItemsXMLStringBuilder.Append("<ITEMCODE>" + code + "</ITEMCODE>");
                                myItemsXMLStringBuilder.Append("<ITEMNAME1>" + product + "</ITEMNAME1>");
                                myItemsXMLStringBuilder.Append("<ITEMNAME2>" + product + "</ITEMNAME2>");
                                myItemsXMLStringBuilder.Append("<QTY>" + quantity + "</QTY>");
                                myItemsXMLStringBuilder.Append("<PRICE>" + price + "</PRICE>");
                                myItemsXMLStringBuilder.Append("<AMT>" + amount + "</AMT>");
                                myItemsXMLStringBuilder.Append("<TAX>" + tx + "</TAX>");
                                myItemsXMLStringBuilder.Append("<TAXR>" + VAT + "</TAXR>");
                                myItemsXMLStringBuilder.Append("</ITEM>");

                                product = "";
                            }
                        }
                    }
                }

                if (line.Contains(ConfigurationManager.AppSettings.Get("InvoiceNumber")))
                {
                    string[] userArray = line.Split(':');
                    InvoiceNumber = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("Cashier")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("Cashier") }, StringSplitOptions.None);
                    cashier = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerEmail")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerEmail") }, StringSplitOptions.None);
                    customerEmail = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CreditNoteRef")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CreditNoteRef") }, StringSplitOptions.None);
                    invoiceComment = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerName")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerName") }, StringSplitOptions.None);
                    customerName = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (cntr == customerAddStart)
                {
                    customerName = line.Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerAdd")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerAdd") }, StringSplitOptions.None);
                    customerAddress = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerTIN")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerTIN") }, StringSplitOptions.None);
                    customerTIN = userArray[userArray.Length - 1].Replace(":", "").Trim();
                    if (customerTIN.Length != 10)
                        customerTIN = String.Empty;
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerVAT")) && !line.Contains("TIN."))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerVAT") }, StringSplitOptions.None);
                    customerVATNumber = userArray[userArray.Length - 1].Replace(":", "").Trim();
                    if (customerVATNumber.Length != 9)
                        customerVATNumber = String.Empty;
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerTEL")))
                {
                    string[] userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerTEL") }, StringSplitOptions.None);
                    CustomerTel = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
            }

            if (mraReceiptLines.Count == 0)
            {
                LogSkipped("NO ITEMS PARSED. productstartline=" + productstartline + " productendline=" + productendline
                    + " ItemDotCounter=" + itemDotCounter
                    + " MultiLineProduct=" + ConfigurationManager.AppSettings.Get("MultiLineProduct"), filepath);
            }

            myItemsXMLStringBuilder.Append("</ITEMS>");

            myCurrenciesXMLStringBuilder.Append("<CurrenciesReceived>");
            myCurrenciesXMLStringBuilder.Append("<Currency>");
            myCurrenciesXMLStringBuilder.Append("<Name>" + currentCurrency + "</Name>");
            myCurrenciesXMLStringBuilder.Append("<Amount>" + invoiceamount + "</Amount>");
            myCurrenciesXMLStringBuilder.Append("<Rate>" + "1" + "</Rate>");
            myCurrenciesXMLStringBuilder.Append("</Currency>");
            myCurrenciesXMLStringBuilder.Append("</CurrenciesReceived>");

            Invoice invoice = new Invoice();

            invoice.InvoiceNumber = InvoiceNumber;
            invoice.InvoiceTaxAmount = Math.Round(overalTax, 2).ToString(Inv);

            if (ConfigurationManager.AppSettings.Get("VatFlag") == "1")
                invoice.InvoiceAmount = Math.Round(overalAmount, 2).ToString(Inv);
            else if (ConfigurationManager.AppSettings.Get("VatFlag") == "0")
                invoice.InvoiceAmount = (Math.Round(overalAmount, 2) + Math.Round(overalTax, 2)).ToString(Inv);

            invoice.Cashier = cashier;
            invoice.BranchName = branchName;
            invoice.Currency = currentCurrency;
            invoice.ItemsXML = myItemsXMLStringBuilder.ToString();
            invoice.CurrenciesXML = myCurrenciesXMLStringBuilder.ToString();
            invoice.CustomerBPN = customerTIN;
            invoice.CustomerName = customerName;
            invoice.CustomerTelephoneNumber = CustomerTel;
            invoice.CustomerVATNumber = customerVATNumber;
            invoice.CustomerAddress = customerAddress;
            invoice.CustomerEmail = customerEmail;
            invoice.Discount = Discount;
            invoice.InvoiceComment = invoiceComment;

            // Build MRA submission data
            invoice.ReceiptLines = mraReceiptLines;
            if (mraReceiptLines.Count > 0)
            {
                var levyBreakdown = mraReceiptLines
                    .GroupBy(r => r.TaxRateId)
                    .Select(g => new
                    {
                        levyTypeId = g.Key,
                        levyRate = g.Key == "A"
                            ? (double)decimal.Parse(ConfigurationManager.AppSettings.Get("VatA") ?? "0.165", Inv)
                            : 0.0,
                        levyAmount = (double)g.Sum(r => r.TotalVAT)
                    }).ToList();
                invoice.LevyBreakDownJson = JsonConvert.SerializeObject(levyBreakdown);
            }
            else
            {
                invoice.LevyBreakDownJson = "[]";
            }

            string invoiceFLG = ConfigurationManager.AppSettings.Get("InvoiceFlag");
            string message = invoiceFLG.Equals("02") ? "Credit Note" : invoiceFLG.Equals("03") ? "Debit Note" : "Normal Invoice";
            invoice.ReasonForCreditNote = invoiceFLG.Equals("01") ? "Invoice" : ConfigurationManager.AppSettings.Get("CreditDebitReason");

            if (!invoiceFLG.Equals("01"))
            {
                if (MessageBox.Show($"Are you sure you want to process {message}", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    invoice.InvoiceFlag = ConfigurationManager.AppSettings.Get("InvoiceFlag");
                }
                else
                {
                    if (MessageBox.Show($"Credit Note? ", "Question?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        invoice.InvoiceFlag = "02";
                        invoice.Discount = Discount;
                    }
                    else
                    {
                        if (MessageBox.Show($"Debit Note? ", "Question?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            invoice.InvoiceFlag = "03";
                            invoice.ReasonForCreditNote = ConfigurationManager.AppSettings.Get("CreditNoteReason");
                        }
                        else
                        {
                            MessageBox.Show("Please set the Invoice Flag in the settings >>> now printing as a Normal invoice");
                            invoice.InvoiceFlag = "01";
                        }
                    }
                }
            }
            else
            {
                invoice.InvoiceFlag = invoiceFLG;
                invoice.InvoiceComment = String.Empty;
            }

            overalAmount = 0;
            overalTax = 0;
            return invoice;
        }

        public void addToConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}