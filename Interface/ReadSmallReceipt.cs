using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Revmax_Interface_Promun
{
    internal class ReadSmallReceipt
    {
        private StringBuilder m_Sb;
        private bool m_bDirty;
        private System.IO.FileSystemWatcher m_Watcher;
        private bool m_bIsWatching;
        //Invoice invoice =  new Invoice();

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
        private string ReduceToEightChars(string input)
        {
            // If the string is less than 8 characters, pad it with spaces on the right
            string paddedString = input.PadRight(8);

            // Take the first 8 characters of the resulting string
            string result = paddedString.Substring(0, 8);

            return result;
        }

        public bool CountDots(int numberOfDots, string line)
        {
            bool dots;
            int counter = 0;
            foreach (char c in line)
            {
                if (c == '.')
                    counter = counter + 1;
            }
            if (counter >= numberOfDots)
                dots = true;
            else
                dots = false;
            return dots;
        }

        public Invoice ReadInvoice(string filepath, string vatFlag, List<ReadCurrencies> CurrenciesList)
        {
            string branchName = "";
            string customerBpn = "";
            string customerName = "";
            string CustomerNum = "";
            //string customerNumber = "null";
            string customerVATNumber = "";
            string invoiceComment = "";
            string invoiceFlag = "01";
            string product = "";
            string CustomerTel = "";
            string customerAddress = "";
            int customerLine = 0;
            int addline = 0;

            int cntinv = 0;
            int productstartline = 0;
            int productendline = 0;
            int invoicenumstartline = 0;
            int invoicenumendline = 0;
            int deliveredstartline = 0;
            int deliveredendline = 0;
            int cntr = 0;
            string quantity;
            string amount;
            string price;
            string code = "";
            int i = 0;
            string customerTIN = "";

            decimal finalTax = 0;
            string invoiceamount = "";
            string invoicetaxamount = "";
            string cashier = "";
            string deliveredto = "";
            string paymentdue = "";
            string ordernumber = "";
            string Discount = "";
            string InvoiceNumber = "";
            string branchname = "", currency = "";
            string currentCurrency = "";
            string tax = "";
            string VAT = "";
            decimal overalAmount = 0;
            decimal overalTax = 0;
            string[] stringsToReplace = { ",", "x", "$", ":", "US", "US$", "D", "ZWL", "ZWD","T" };
            StringBuilder myItemsXMLStringBuilder = new StringBuilder();
            myItemsXMLStringBuilder.Append("<ITEMS>");
            StringBuilder myCurrenciesXMLStringBuilder = new StringBuilder();
            List<item> items = new List<item>();
            // finding the area of price quantity and price
            foreach (string kline in File.ReadAllLines(filepath))
            {
                cntinv += 1;
                if (kline.Contains(ConfigurationManager.AppSettings.Get("ProductStartLine2")))
                    productstartline = cntinv;

                if (kline.Contains(ConfigurationManager.AppSettings.Get("ProductEndLine2")))
                    productendline = cntinv;

                for (int a = 0; a < CurrenciesList.Count; a++)
                {
                    if (kline.Contains(CurrenciesList[a].Keyword))
                        currentCurrency = CurrenciesList[a].Name;
                }

                if (currentCurrency == "")
                {
                    currentCurrency = ConfigurationManager.AppSettings.Get("DefaultCurrency");
                }

                /*if (kline.ToLower().Contains("credit note"))
                {
                    addToConfig("InvoiceFlag", "02");
                }
                else if (kline.ToLower().Contains("fiscal tax invoice"))
                {
                    addToConfig("InvoiceFlag", "01");
                }*/
                addToConfig("InvoiceFlag", "01");


            }


            /*var textPath = "product.txt";
            StreamWriter ry = new StreamWriter(textPath);*/

            foreach (string line in File.ReadAllLines(filepath))
            {

                cntr += 1;

                if (ConfigurationManager.AppSettings.Get("MultiLineProduct") == "1")
                {

                    if ((cntr > productstartline && cntr < productendline) & CountDots(int.Parse(ConfigurationManager.AppSettings.Get("ItemDotCounter")), line) == false & line.Contains("Price") == false & line.Contains("----------") == false & line.Equals("") == false)
                    {

                        product = line;

                        //  MessageBox.Show(line);

                    }
                    else if ((cntr > productstartline && cntr < productendline) & CountDots(int.Parse(ConfigurationManager.AppSettings.Get("ItemDotCounter")), line) == true)
                    {

                        i += 1;
                        string[] arrayprod = line.Split(' ');
                        string tempStr = string.Join("`", arrayprod);
                        arrayprod = null;
                        arrayprod = tempStr.Split(new char[] { '`' }, StringSplitOptions.RemoveEmptyEntries);
                        code = ".";
                        quantity = arrayprod[arrayprod.Length - int.Parse(ConfigurationManager.AppSettings.Get("ColumnQuantityIndex"))];
                        quantity = quantity.Replace("$", "");
                        quantity = quantity.Replace(",", "");
                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("TaxSymbol")) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("NonTaxSymbol")))
                        {
                            quantity = quantity.Replace(ConfigurationManager.AppSettings.Get("NonTaxSymbol"), "");
                            quantity = quantity.Replace(ConfigurationManager.AppSettings.Get("TaxSymbol"), "");
                        }
                        quantity = quantity.Trim();
                        // MessageBox.Show(quantity);

                        price = arrayprod[arrayprod.Length - int.Parse(ConfigurationManager.AppSettings.Get("ColumnPriceIndex"))];
                        price = price.Replace("$", "");
                        price = price.Replace("$", "");
                        price = price.Replace("US$", "");
                        price = price.Replace("USD$", "");
                        price = price.Replace("T1", "");

                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("TaxSymbol")) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("NonTaxSymbol")))
                        {
                            price = price.Replace(ConfigurationManager.AppSettings.Get("NonTaxSymbol"), "");
                            price = price.Replace(ConfigurationManager.AppSettings.Get("TaxSymbol"), "");
                        }
                        price = price.Trim();
                        // MessageBox.Show(price);

                        amount = arrayprod[arrayprod.Length - int.Parse(ConfigurationManager.AppSettings.Get("ColumnAmountIndex"))];
                        amount = amount.Replace("$", " ");
                        amount = amount.Replace(",", "");
                        amount = amount.Replace("T", "");
                        amount = amount.Replace("US", "");
                        amount = amount.Replace("US$", "");

                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("TaxSymbol")) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("NonTaxSymbol")))
                        {
                            amount = amount.Replace(ConfigurationManager.AppSettings.Get("TaxSymbol"), "");
                            amount = amount.Replace(ConfigurationManager.AppSettings.Get("NonTaxSymbol"), "");
                        }


                        amount = amount.Trim();
                        if ((Convert.ToDecimal(amount) == 0) || (Convert.ToDecimal(quantity) == 0))
                        {
                            continue;
                        }
                        //  MessageBox.Show(amount);
                        if (String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("TaxSymbol")) && String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("NonTaxSymbol")))
                        {
                            VAT = ConfigurationManager.AppSettings.Get("VatA");
                            tax = VAT.ToString();
                        }

                        else
                        {


                            if (arrayprod[arrayprod.Length - 1].Contains(ConfigurationManager.AppSettings.Get("TaxSymbol")))
                            {
                                VAT = ConfigurationManager.AppSettings.Get("VatA");
                                tax = VAT.ToString();
                            }
                            else if (arrayprod[arrayprod.Length - 1].Contains(ConfigurationManager.AppSettings.Get("NonTaxSymbol")))

                            {


                                VAT = ConfigurationManager.AppSettings.Get("VatE");
                                tax = VAT.ToString();
                            }





                        }

                        string taxamount = "";
                        //   taxamount = Convert.ToString(Convert.ToDecimal(amount) * Convert.ToDecimal(tax));//2.223

                        string tx = "";

                        if (ConfigurationManager.AppSettings.Get("VatFlag") == "1")
                            tx = Convert.ToString(Convert.ToDecimal(amount) - Convert.ToDecimal(amount) / (1 + Convert.ToDecimal(tax)));
                        else if (ConfigurationManager.AppSettings.Get("VatFlag") == "0")
                            tx = Convert.ToString(Convert.ToDecimal(amount) * Convert.ToDecimal(tax)); ;

                        //MessageBox.Show("tax: " + tx);
                        product = product.Replace("!", " ").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "").Replace("~", "").Replace("<", "").Replace(">", "").Replace("?", "").Replace("/", "").Replace(@"\", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("|", "").Replace("`", "").Trim();

                        items.Add(new item
                        {
                            HH = (i + 1).ToString(),
                            ITEMCODE = code,
                            ITEMNAME1 = product,
                            ITEMNAME2 = product,
                            Quantity = quantity,
                            Price = price,
                            Amount = amount,
                            Tax = tax,
                            TaxR = VAT

                        });
                        ;


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

                    }

                }
                else
                {
                    if ((cntr > productstartline && cntr < productendline) & CountDots(int.Parse(ConfigurationManager.AppSettings.Get("ItemDotCounter2")), line) == true && !(line.Contains("Qty.") || line.Contains("Price.") || line.Contains("Amt.")))
                    {


                        i += 1;
                        string[] arrayprod = line.Split(' ');
                        string tempStr = string.Join("`", arrayprod);
                        arrayprod = null;
                        arrayprod = tempStr.Split(new char[] { '`' }, StringSplitOptions.RemoveEmptyEntries);

                        code = ".";
                        //process a product
                        for (int m = 2; m < arrayprod.Length - 2; m++)
                        {
                            product += " " + arrayprod[m];
                        }

                        //process a quantity
                        quantity = arrayprod[0];
                        quantity = quantity.Replace("$", "").Replace(",", "").Trim();

                        amount = arrayprod[arrayprod.Length - 1];
                        /*amount = amount.Replace("$", " ").Replace(",", "").Replace("USD", "").Trim();*/
                        amount = stringsToReplace.Aggregate(amount, (current, str) => current.Replace(str, "")).Trim();

                        price = Convert.ToString(Convert.ToDecimal(amount) / Convert.ToDecimal(quantity));

                        if (arrayprod[arrayprod.Length - 1].Contains("T"))
                        {
                            VAT = ConfigurationManager.AppSettings.Get("VatA");
                            tax = VAT.ToString();
                        }
                        else
                        {
                            VAT = ConfigurationManager.AppSettings.Get("VatE");
                            tax = VAT.ToString();
                        }

                        string taxamount = "";
                        taxamount = Convert.ToString(Convert.ToDecimal(amount) * Convert.ToDecimal(tax));//2.223

                        string tx = "";

                        if (ConfigurationManager.AppSettings.Get("VatFlag2") == "1")
                            tx = Convert.ToString(Math.Round(Convert.ToDecimal(amount) - Convert.ToDecimal(amount) / (1 + Convert.ToDecimal(tax)), 2));
                        else if (ConfigurationManager.AppSettings.Get("VatFlag2") == "0")
                            tx = taxamount;

                        product = product.Replace("!", " ").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "").Replace("~", "").Replace("<", "").Replace(">", "").Replace("?", "").Replace("/", "").Replace(@"\", "").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("|", "").Replace("`", "").Trim();
                        overalTax += Convert.ToDecimal(tx);
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
                        ;
                        overalAmount = overalAmount += Convert.ToDecimal(amount);

                        //MessageBox.Show("Code: "  + product+  "Quantity: "  +quantity + "Price: " + price + "Amount: " + amount + "Tax: " + tx);
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

                if (line.Contains(ConfigurationManager.AppSettings.Get("InvoiceNumber2")))
                {
                    string[] userArray;
                    userArray = line.Split(' ');
                    InvoiceNumber = userArray[1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("Cashier")))
                {
                    string[] userArray;
                    userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("Cashier") }, StringSplitOptions.None);
                    cashier = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }

                if (line.Contains(ConfigurationManager.AppSettings.Get("CreditNoteRef")))
                {
                    string[] userArray;
                    userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CreditNoteRef") }, StringSplitOptions.None);
                    invoiceComment = userArray[userArray.Length - 1].Replace(":", "").Trim();
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerName")))
                {
                    customerLine = cntr + 2;
                    addline = cntr + 3;

                }
                if (cntr == customerLine)
                {

                    customerName = line
                        .Replace("O.", "")
                        .Replace("P.", "")
                        .Replace("BOX", "")
                        .Replace("Private Bag", "")
                        .Replace("P. Bag", "")
                        .Replace("P Bag", "")
                        .Trim();

                    string RemoveNumbers(string input)
                    {
                        StringBuilder result = new StringBuilder();
                        foreach (char c in input)
                        {
                            if (!char.IsDigit(c))
                            {
                                result.Append(c);
                            }
                        }
                        return result.ToString().Trim();
                    }
                    customerName = RemoveNumbers(customerName);
                    //MessageBox.Show($"Customer Name: {customerName}");
                }
                if (cntr == addline)
                {
                    customerAddress = line.Trim();
                    //MessageBox.Show($"Customer Address: {customerAddress}");
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerTIN")))
                {
                    string[] userArray;
                    userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerTIN") }, StringSplitOptions.None);
                    customerTIN = userArray[userArray.Length - 1].Replace(":", "").Trim();
                    if (customerTIN.Length != 10)
                    {
                        customerTIN = String.Empty;
                    }
                    // MessageBox.Show($"Customer TIN: {customerTIN}");
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerVAT")) && !line.Contains("TIN."))
                {
                    string[] userArray;
                    userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerVAT") }, StringSplitOptions.None);
                    customerVATNumber = userArray[userArray.Length - 1].Replace(":", "").Trim();
                    if (customerVATNumber.Length != 9)
                    {
                        customerVATNumber = String.Empty;
                    }
                    //MessageBox.Show($"Customer VAT: {customerVATNumber}");
                }
                if (line.Contains(ConfigurationManager.AppSettings.Get("CustomerTEL")))
                {
                    string[] userArray;
                    userArray = line.Split(new string[] { ConfigurationManager.AppSettings.Get("CustomerTEL") }, StringSplitOptions.None);
                    CustomerTel = userArray[userArray.Length - 1].Replace(":", "").Trim();
                    //MessageBox.Show($"Customer TEL: {CustomerTel}");
                }

            }
            myItemsXMLStringBuilder.Append("</ITEMS>");


            myCurrenciesXMLStringBuilder.Append("<CurrenciesReceived>");
            myCurrenciesXMLStringBuilder.Append("<Currency>");
            myCurrenciesXMLStringBuilder.Append("<Name>" + currentCurrency + "</Name>");
            myCurrenciesXMLStringBuilder.Append("<Amount>" + invoiceamount + "</Amount>");
            myCurrenciesXMLStringBuilder.Append("<Rate>" + "1" + "</Rate>");
            myCurrenciesXMLStringBuilder.Append("</Currency>");
            myCurrenciesXMLStringBuilder.Append("</CurrenciesReceived>");
            // MessageBox.Show(myCurrenciesXMLStringBuilder.ToString());
            //MessageBox.Show(myItemsXMLStringBuilder.ToString());


            Invoice invoice = new Invoice();

            invoice.InvoiceNumber = InvoiceNumber;
            //MessageBox.Show($"Invoice Number: {InvoiceNumber}");
            /*MessageBox.Show($"Invoice Number: {InvoiceNumber}");
            MessageBox.Show($"Invoice Comment: {invoiceComment}");
            MessageBox.Show($"Invoice Amount: {Math.Round(overalAmount, 2).ToString()}");
            MessageBox.Show($"Invoice Tax: {Math.Round(overalTax, 2).ToString()}");
            MessageBox.Show($"Invoice flag: {ConfigurationManager.AppSettings.Get("InvoiceFlag")}");*/

            invoice.InvoiceTaxAmount = Math.Round(overalTax, 2).ToString();
            if (ConfigurationManager.AppSettings.Get("VatFlag2") == "1")
            {
                invoice.InvoiceAmount = Math.Round(overalAmount, 2).ToString();
            }

            else if (ConfigurationManager.AppSettings.Get("VatFlag2") == "0")
            {
                invoice.InvoiceAmount = (Math.Round(overalAmount, 2) + Math.Round(overalTax, 2)).ToString();
            }


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

            invoice.Discount = Discount;
            invoice.InvoiceComment = invoiceComment;

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
            //MessageBox.Show($"Invoice Flag{invoice.InvoiceFlag}");
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



        ///////////////////////////////////////////end transact//////////////////////////////////////
        ///
    }
}
