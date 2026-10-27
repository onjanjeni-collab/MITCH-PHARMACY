
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using MRAAPI;
using Revmax_Interface_Promun.Processing;
using Revmax_Interface_Promun.Services;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.ServiceProcess;
using System.Drawing.Imaging;
using System.Web.Services.Description;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Threading;
using Formatting = Newtonsoft.Json.Formatting;

namespace Revmax_Interface_Promun
{


    public partial class RevMaxInterfaceWizard : Form
    {
        private static MraLib _mra;
        private static readonly MRALibTest _mraLib = new MRALibTest();
        private bool isDeviceConfigRefreshInProgress = false;
        private readonly System.Windows.Forms.Timer deviceConfigRefreshTimer = new System.Windows.Forms.Timer();
        public static List<TerminalSiteProduct> CachedSiteProducts { get; private set; } = new List<TerminalSiteProduct>();

        string Currency = "";
        string BranchName = "";
        string InvoiceNumber = "";
        string Discount = "";
        string CustomerName = "";
        string CustomerNumber = "";
        string ReceiptGlobalNumber = "";
        string CustomerVATNumber = "";
        string CustomerAddress = "";
        string CustomerTelephoneNumber = "";
        string CustomerBPN = "";
        string InvoiceAmount = "";
        string InvoiceFlag = "";
        string Cashier = "";
        string InvoiceComment = "";
        string ItemsXML = "";
        string CurrenciesXML = "";
        string InvoiceTaxAmount = "";
        string code = "";
        string Currencies = "";
        string Signature = "";
        string QRCode = "";
        string _printNotes = "";
        RootObject rootinv = new RootObject();
        Invoice printInvoice = new Invoice();
        List<item> list = new List<item>();
        Convolve convolve = new Convolve();
        ReadSmallReceipt readSmallReceipt = new ReadSmallReceipt();
        private bool isProcessDayStatusExecuted = false;

        XmlDocument xmldoc1 = new XmlDocument();
        string receiptPath = "";

        List<ReadCurrencies> CurrenciesList = new List<ReadCurrencies>();

        CardDetails CardDetails = new CardDetails();




        public void ProcessDayStatus()
        {
            // ZReport not supported with MRA — no-op
        }


        private void UpdateErrorsLabel(string error)
        {
            // Update the label controller errors.text with the error message
            // Assuming there is a label named "errors" in the UI controller
            //ZErrors.Text = error;
        }

        public RevMaxInterfaceWizard()
        {
            InitializeComponent();

            this.reportViewer1.RefreshReport();

            _mra = new MraLib();
            LoadCachedSiteProductsFromDisk();

            CardDetails = new CardDetails
            {
                Data = new Data1
                {
                    CompanyName = ConfigurationManager.AppSettings.Get("CompanyName") ?? "",
                    TIN = ConfigurationManager.AppSettings.Get("CompanyTIN") ?? "",
                    VAT = ConfigurationManager.AppSettings.Get("CompanyVAT") ?? "",
                    RegistrationNumber = ConfigurationManager.AppSettings.Get("CompanyRegNum") ?? "",
                    SerialNumber = ConfigurationManager.AppSettings.Get("DeviceSerial") ?? ""
                }
            };

            InitializeDeviceConfigRefreshTimer();
            InitialiseDeviceAsync();

            addToConfig("Message", $"Interface Started: {DateTime.Now.ToString()}");
            CheckAndUnzipFolders();
            timer2.Stop();
            timer2.Start();



            //MessageBox.Show((CardDetails) JsonConvert.DeserializeObject<CardDetails>(j));

            //MessageBox.Show(printInvoice.Cashier);



            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory.ToString() + "CurConf.interface"))
            {
                using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory.ToString() + "CurConf.interface", FileMode.Open, FileAccess.Read))
                {
                    XmlDataDocument xmldoc1 = new XmlDataDocument();
                    XmlNodeList xmlnode1;
                    XmlNodeList xmlnode2;


                    try
                    {
                        xmldoc1.Load(fs);
                        xmlnode1 = xmldoc1.GetElementsByTagName("currency");

                        for (int i = 0; i <= xmlnode1.Count - 1; i++)
                        {
                            CurrenciesList.Add(new ReadCurrencies
                            {
                                Keyword = xmlnode1[i].ChildNodes.Item(0).InnerText.Trim(),
                                Name = xmlnode1[i].ChildNodes.Item(1).InnerText.Trim(),
                            });

                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            else
            {
                MessageBox.Show("Currency Config not found.Contact Axis Solutions.");
            }

        }

        private void InitializeDeviceConfigRefreshTimer()
        {
            deviceConfigRefreshTimer.Interval = 5 * 60 * 1000;
            deviceConfigRefreshTimer.Tick += async (s, e) => await RefreshDeviceConfigAndUpdateVatAsync("timer");
            deviceConfigRefreshTimer.Start();
        }

        private async void InitialiseDeviceAsync()
        {
            await RefreshDeviceConfigAndUpdateVatAsync("startup");
        }

        private async Task RefreshDeviceConfigAndUpdateVatAsync(string trigger)
        {
            if (isDeviceConfigRefreshInProgress) return;
            isDeviceConfigRefreshInProgress = true;
            try
            {
                try
                {
                    var initResponse = await _mraLib.InitializeAsync();
                    var taxRates = initResponse?.Data?.Configuration?.GlobalConfiguration?.TaxRates;
                    if (taxRates != null && taxRates.Count > 0)
                    {
                        foreach (var taxRate in taxRates)
                        {
                            decimal rateDecimal = taxRate.Rate / 100m;
                            string rateValue = rateDecimal.ToString("0.000");
                            switch (taxRate.Id?.Trim())
                            {
                                case "A": addToConfig("VatA", rateValue); break;
                                case "B": addToConfig("VatE", rateValue); break;
                            }
                        }
                    }

                    if (initResponse?.Data?.TaxpayerDetails?.Tin != null)
                    {
                        addToConfig("CompanyTIN", initResponse.Data.TaxpayerDetails.Tin);
                        CardDetails.Data.TIN = initResponse.Data.TaxpayerDetails.Tin;
                    }
                }
                catch (Exception initEx)
                {
                    System.Diagnostics.Debug.WriteLine($"InitializeAsync error ({trigger}): {initEx.Message}");
                }

                try
                {
                    var productsResult = await _mra.GetTerminalSiteProductsAsync();
                    if (productsResult != null && productsResult.StatusCode == 1 && productsResult.Data != null)
                    {
                        CachedSiteProducts = productsResult.Data.Select(p => new TerminalSiteProduct
                        {
                            ProductCode = p.ProductCode,
                            ProductName = p.ProductName,
                            Description = p.Description,
                            Quantity = p.Quantity,
                            UnitOfMeasure = p.UnitOfMeasure,
                            Price = p.Price,
                            SiteId = p.SiteId,
                            MinimumStockLevel = p.MinimumStockLevel,
                            TaxRateId = p.TaxRateId,
                            IsProduct = p.IsProduct
                        }).ToList();
                        File.WriteAllText(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.json"),
                            JsonConvert.SerializeObject(productsResult, Newtonsoft.Json.Formatting.Indented),
                            Encoding.UTF8);
                    }
                    else
                    {
                        LoadCachedSiteProductsFromDisk();
                    }
                }
                catch (Exception prodEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Products fetch error ({trigger}): {prodEx.Message}");
                    LoadCachedSiteProductsFromDisk();
                }
            }
            finally
            {
                isDeviceConfigRefreshInProgress = false;
            }
        }

        private static void LoadCachedSiteProductsFromDisk()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.json");
                if (!File.Exists(path)) return;
                var json = File.ReadAllText(path, Encoding.UTF8);
                var jObj = Newtonsoft.Json.Linq.JObject.Parse(json);
                var dataToken = jObj["data"];
                if (dataToken == null) return;
                var products = dataToken.ToObject<List<TerminalSiteProduct>>();
                if (products != null && products.Count > 0)
                    CachedSiteProducts = products;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load products.json: {ex.Message}");
            }
        }

        public void CheckAndUnzipFolders()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string x86Folder = Path.Combine(currentDirectory, "x86");
            string x64Folder = Path.Combine(currentDirectory, "x64");

            if (!Directory.Exists(x86Folder) || !Directory.Exists(x64Folder))
            {
                UnzipFolders(currentDirectory);
            }
            else
            {
                Console.WriteLine("x86 and x64 folders already exist.");
            }
        }

        private void UnzipFolders(string destinationPath)
        {
            string zipFilePathX86 = Path.Combine(destinationPath, "x86.zip");
            string zipFilePathX64 = Path.Combine(destinationPath, "x64.zip");

            if (File.Exists(zipFilePathX86))
            {
                ZipFile.ExtractToDirectory(zipFilePathX86, destinationPath);
                Console.WriteLine("x86 folder unzipped successfully.");
            }
            else
            {
                Console.WriteLine("x86.zip file not found.");
            }

            if (File.Exists(zipFilePathX64))
            {
                ZipFile.ExtractToDirectory(zipFilePathX64, destinationPath);
                Console.WriteLine("x64 folder unzipped successfully.");
            }
            else
            {
                Console.WriteLine("x64.zip file not found.");
            }
        }

        public void CopyCompanyLogo(string logoDir)
        {
            string basePath = Application.StartupPath; // Get the base directory of the application

            // Check if the Logo directory exists
            if (!Directory.Exists(logoDir))
            {
                // If it doesn't exist, create it
                Directory.CreateDirectory(logoDir);
            }

            // Get all files named "companyLogo" with any extension in the base directory
            string[] companyLogoFiles = Directory.GetFiles(basePath, "companyLogo.*");

            if (companyLogoFiles.Length > 0)
            {
                // Get the path of the first companyLogo file found
                string companyLogoFilePath = companyLogoFiles[0];

                // Get the extension of the companyLogo file
                string companyLogoExtension = Path.GetExtension(companyLogoFilePath);

                // Construct the destination file path in the Logo directory
                string destinationLogoFilePath = Path.Combine(logoDir, "logo" + companyLogoExtension);

                // Check if there are existing logo files in the destination directory
                string[] existingLogoFiles = Directory.GetFiles(logoDir, "logo.*");

                if (existingLogoFiles.Length == 0)
                {
                    // Copy companyLogo file to the Logo directory and rename it to "logo"
                    File.Copy(companyLogoFilePath, destinationLogoFilePath, true);
                }
                else
                {
                    // Inform the user that there's already a logo file in the destination directory
                    // You can choose to handle this differently based on your application's requirements
                    //Console.WriteLine("A logo file already exists in the destination directory.");
                }
            }
            else
            {
                //Console.WriteLine("No companyLogo file found in the root directory.");
            }
        }


        public void PrintTextUnficalised(string filePath)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (sender, e) =>
                {
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(filePath))
                    {
                        string text = streamReader.ReadToEnd();
                        e.Graphics.DrawString(text, new System.Drawing.Font("Arial", 12), System.Drawing.Brushes.Black, 2, e.MarginBounds.Top);
                    }
                };

                pd.PrinterSettings.PrinterName = GetPrinterName();
                pd.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while printing the text file: " + ex.Message);
            }
        }

        private string GetPrinterName()
        {
            // Check if the configuration specifies to use a specific printer
            if (ConfigurationManager.AppSettings.Get("setPrinter")?.ToLower() == "true")
            {
                return ConfigurationManager.AppSettings.Get("PrinterName");
            }
            else
            {
                // Use the default printer if not specified in the configuration
                return new PrinterSettings().PrinterName;
            }
        }
        public void DailyReport(bool isOpen)
        {
            DateTime today = DateTime.Now;
            string lDate = ConfigurationManager.AppSettings.Get("LastZReportDate");

            if (String.IsNullOrEmpty(lDate))
            {
                lDate = DateTime.Now.ToString();
            }

            // Parse lDate to DateTime
            DateTime lastDate;
            if (!DateTime.TryParse(lDate, out lastDate))
            {
                lastDate = DateTime.Now;
            }

            // Check if it's a new day
            if (today.Date == lastDate.Date)
            {
                // Store variables
                bool closed = false;

                // Check if it's 12 PM and status is open
                if (today.Hour == 7 && today.Minute == 49 && isOpen)
                {
                    // Call ZReport with closed set to true (possibly closing report)
                    ZReading();
                    ZReading();
                    closed = false; // Update closed flag
                }

                // Check if it's 12 AM and status is closed
                if (today.Hour == 7 && today.Minute == 49 && !isOpen)
                {
                    // Call ZReport with closed set to false (possibly opening report)
                    ZReading();
                    closed = false; // Update closed flag
                }

                // Update last date for future comparisons
                addToConfig("LastZReportDate", today.ToString());
            }
            else
            {
                return; // Indicate it's not a new day
            }
        }

               


        private async void timer1_Tick(object sender, EventArgs e)
        {
            string basePath = ConfigurationManager.AppSettings.Get("BasePath");
            string targetDir2 = ConfigurationManager.AppSettings.Get("targetDir2");

            if (ConfigurationManager.AppSettings.Get("AutomateZReport") == "true")
            {

                DateTime now = DateTime.Now;
                int hour = now.Hour;
                int minute = now.Minute;

                if (!(hour == 23 && minute >= 50 && minute <= 59))
                {
                    isProcessDayStatusExecuted = false;
                }
                else if (hour == 23 && minute >= 50 && minute <= 59 && !isProcessDayStatusExecuted)
                {
                    //MessageBox.Show("zzzzzz");
                    ProcessDayStatus();
                    isProcessDayStatusExecuted = true; // Set the flag to indicate that ProcessDayStatus has been executed
                }

            }
            


            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!Directory.Exists(ConfigurationManager.AppSettings.Get("ReceiptFolder")))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("ReceiptFolder"));
            }
            Directory.CreateDirectory($"{basePath}\\Receipts");
            Directory.CreateDirectory($"{basePath}\\Backup");
            Directory.CreateDirectory($"{basePath}\\Logo");
            Directory.CreateDirectory($"{basePath}\\NonReceipts");
            Directory.CreateDirectory(targetDir2);

            //string sourceDir = Path.Combine(basePath, "Receipts");
            string sourceDir = ConfigurationManager.AppSettings.Get("UseCustomReceiptFolder") == "true" ? ConfigurationManager.AppSettings.Get("ReceiptFolder") : Path.Combine(basePath, "Receipts");
            string targetDir = Path.Combine(basePath, "Backup");
            string logoDir = Path.Combine(basePath, "Logo");
            string nonReceiptsDir = Path.Combine(basePath, "NonReceipts");

            CopyCompanyLogo(logoDir);
            timer1.Stop();

            // convolve.Send("C:\\Users\\obeyn\\Documents\\PROJECTS\\AXIS SOLUTIONS\\INTERFACES\\V2\\Sandvik\\Sandvik Receipts and documentation\\sandvik\\2020-11 unsigned\\2020-11-02 10.17.36.8568_INVOICE_I211678_64721741.txt");
            foreach (var receipt in Directory.GetFiles(sourceDir))
            {
                //MessageBox.Show(receipt.ToString());
                receiptPath = receipt.ToString();
                //if receiptpath name is not Receipt.txt and if ConfigurationManager.AppSettings.Get("ReadSpecificFilename") is true then skip the file
                if(ConfigurationManager.AppSettings.Get("ReadSpecificFilename") == "true" && !Path.GetFileName(receiptPath).Equals(ConfigurationManager.AppSettings.Get("SpecificFilename")))
                {
                    continue;
                }   

                bool isFileReady = false;
                while (!isFileReady)
                {
                    try
                    {
                        // Attempt to open the file to check if it is in use
                        using (var fileStream = File.Open(receiptPath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            isFileReady = true; // File is not in use
                        }
                    }
                    catch (IOException)
                    {
                        // File is in use; wait and retry
                        Thread.Sleep(700); // Delay time in milliseconds
                    }
                }
                string data = File.ReadAllText(receiptPath);
                //MessageBox.Show($"Indentifier:  {ConfigurationManager.AppSettings.Get("InvoiceIdentifierText")}");
                switch (data.Contains(ConfigurationManager.AppSettings.Get("InvoiceIdentifierText")))
                {
                    case true:
                        if (!data.Contains(ConfigurationManager.AppSettings.Get("InvoiceIdentifierText")))
                        {
                            //MessageBox.Show("testtt 1");
                            PrintDumpFile(receiptPath);
                            //File.Delete(receiptPath);
                            File.Move(receiptPath, $"{nonReceiptsDir}\\receipt{DateTime.Now.ToString("yyyy_MM_dd HHmmss")}");
                            //MessageBox.Show("after");
                            receiptPath = "";
                        }
                        else
                        {
                            //MessageBox.Show(receiptPath.ToString());

                            bool fiscalized1 = await FiscalizeAsync(convolve.ReadInvoice(receipt, ConfigurationManager.AppSettings.Get("VatFlag"), CurrenciesList));
                            if (fiscalized1)
                            {
                                try
                                {
                                    using (StreamWriter appen = File.AppendText(receipt))
                                    {
                                        appen.WriteLine($"\n \n QR Code: {printInvoice.QRCode}");
                                        appen.WriteLine($"\n Verification Code: {printInvoice.VerificationCode}");
                                    }
                                }
                                catch { }
                            }

                            Print("FISCAL TAX INVOICE");
                            if (ConfigurationManager.AppSettings.Get("NumberOfCopies").Equals("2"))
                            {
                                Print("COPY OF FISCAL TAX INVOICE");
                            }
                            list.Clear();
                        }
                        break;
                    case false:
                        //MessageBox.Show("testtt 2");
                        if (!data.Contains(ConfigurationManager.AppSettings.Get("InvoiceIdentifierText2")))
                        {
                            //File.Delete(receiptPath);
                            PrintDumpFile(receiptPath);
                            File.Move(receiptPath, $"{nonReceiptsDir}\\receipt{DateTime.Now.ToString("yyyy_MM_dd HHmmss")}");
                            //MessageBox.Show("after");
                            receiptPath = "";
                        }
                        else
                        {
                            //MessageBox.Show(receiptPath.ToString());

                            bool fiscalized2 = await FiscalizeAsync(readSmallReceipt.ReadInvoice(receipt, ConfigurationManager.AppSettings.Get("VatFlag"), CurrenciesList));
                            if (fiscalized2)
                            {
                                try
                                {
                                    using (StreamWriter appen = File.AppendText(receipt))
                                    {
                                        appen.WriteLine($"\n \n QR Code: {printInvoice.QRCode}");
                                        appen.WriteLine($"\n Verification Code: {printInvoice.VerificationCode}");
                                    }
                                }
                                catch { }
                            }

                            Print("FISCAL INVOICE");

                            if (false)
                            {
                                //PrintTextUnficalised(receipt);
                            }
                            list.Clear();
                        }
                        break;
                    default:
                        break;
                }



                if (!String.IsNullOrEmpty(receiptPath))
                {
                    string fiscalStatus = string.IsNullOrEmpty(_printNotes) ? "Success" : "Failed";
                    string backupBaseName;
                    if (ConfigurationManager.AppSettings.Get("UseReceiptFilenameForBackup") == "true")
                    {
                        string lineKey = ConfigurationManager.AppSettings.Get("BackupFilenameKey") ?? "";
                        backupBaseName = printInvoice?.InvoiceNumber ?? "Unknown";
                        if (!string.IsNullOrWhiteSpace(lineKey))
                        {
                            foreach (var line in data.Split('\n'))
                            {
                                if (line.Contains(lineKey))
                                {
                                    string extracted = line
                                        .Substring(line.IndexOf(lineKey) + lineKey.Length)
                                        .Replace(":", "").Trim();
                                    if (!string.IsNullOrWhiteSpace(extracted))
                                    {
                                        backupBaseName = extracted;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        backupBaseName = (printInvoice?.InvoiceNumber ?? "Unknown")
                            .Replace("/", "-").Replace("\\", "-").Replace(":", "-").Replace(" ", "_").Trim();
                    }
                    backupBaseName = backupBaseName
                        .Replace("/", "-").Replace("\\", "-").Replace(":", "-").Replace(" ", "_").Trim();
                    File.Copy(receiptPath, Path.Combine(targetDir, $"{fiscalStatus} {backupBaseName}.pdf"), true);
                   // MessageBox.Show(targetDir2 + "\\" + receiptPath.Replace(ConfigurationManager.AppSettings.Get("ReceiptFolder"), ""));
                    //File.Copy(receiptPath, targetDir2 + "\\" + receiptPath.Replace(ConfigurationManager.AppSettings.Get("ReceiptFolder"), ""));
                    try
                    {
                        //MessageBox.Show("before2");
                        File.Delete(receiptPath);
                        //MessageBox.Show("after 2");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("We could not delete the file.Please check the file permissions." + ex.Message);
                    }
                }

                receiptPath = "";
                timer1.Start();


                // invoice = readFile.ReadInvoice(receipt, ConfigurationManager.AppSettings.Get("VatFlag"), CurrenciesList);
            }
            /*
                        if (!String.IsNullOrEmpty(receiptPath))
                        {
                            File.Copy(receiptPath, targetDir + "\\REVMAX_" + DateTime.Now.ToString("dd_mm_yyyy_HH_MM_ss") + ".txt");
                            File.Copy(receiptPath, targetDir2 + "\\" + receiptPath.Replace(ConfigurationManager.AppSettings.Get("ReceiptFolder"), ""));
                            try
                            {
                                //MessageBox.Show("before2");
                                File.Delete(receiptPath);
                                //MessageBox.Show("after 2");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("We could not delete the file.Please check the file permissions." + ex.Message);
                            }
                        }

                        receiptPath = "";
                        timer1.Start();*/
                  timer1.Start();

        }



        public void PrintDumpFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("The specified file was not found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PrintDocument printDocument = new PrintDocument();
            string defaultPrinter = new PrinterSettings().PrinterName;

            if (string.IsNullOrEmpty(defaultPrinter))
            {
                MessageBox.Show("No default printer is set.", "Printer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            printDocument.PrinterSettings.PrinterName = defaultPrinter;
            printDocument.DocumentName = Path.GetFileName(filePath);

            printDocument.PrintPage += (sender, e) => PrintPageHandler2(sender, e, filePath);

            try
            {
                printDocument.Print();
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"An error occurred while printing:\n{ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void PrintPageHandler2(object sender, PrintPageEventArgs e, string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string text = reader.ReadToEnd();
                using (Font font = new Font("Arial", 9))
                {
                    // Set left margin to zero
                    RectangleF printArea = new RectangleF(0, e.MarginBounds.Top, e.MarginBounds.Width + e.MarginBounds.Left, e.MarginBounds.Height);
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,  // Ensures left alignment
                        LineAlignment = StringAlignment.Near
                    };

                    e.Graphics.DrawString(text, font, Brushes.Black, printArea, format);
                }
            }
        }





        private void Print(String ReceiptLabel)
        {
            List<item> itemlist = new List<item>();
            List<TaxTable> taxTables = new List<TaxTable>();

            // Build tax table from invoice lines prepared by Convolve
            decimal vatARate3 = 0;
            decimal.TryParse(ConfigurationManager.AppSettings.Get("VatA") ?? "0.165",
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out vatARate3);
            var printTaxes = BuildReceiptTaxesFromLines(
                printInvoice.ReceiptLines ?? new List<MraReceiptLine>(), vatARate3);

            foreach (var taxItem in printTaxes)
            {
                if (taxItem.TaxPercent == 0 && taxItem.TaxCode == "B")
                {
                    taxTables.Add(new TaxTable
                    {
                        TaxPercent = "0",
                        NetAmount = taxItem.SalesAmountWithTax - taxItem.TaxAmount,
                        TaxAmount = taxItem.TaxAmount,
                        SalesAmountWithTax = taxItem.SalesAmountWithTax
                    });
                }
                if (taxItem.TaxCode == "A")
                {
                    taxTables.Add(new TaxTable
                    {
                        TaxPercent = taxItem.TaxPercent.ToString(),
                        NetAmount = taxItem.SalesAmountWithTax - taxItem.TaxAmount,
                        TaxAmount = taxItem.TaxAmount,
                        SalesAmountWithTax = taxItem.SalesAmountWithTax
                    });
                }
            }

            try
            {
                foreach (var line in printInvoice.ReceiptLines ?? new List<MraReceiptLine>())
                {
                    itemlist.Add(new item()
                    {
                        ITEMNAME1 = line.Description ?? "",
                        Quantity = line.Quantity.ToString(),
                        Price = line.UnitPrice.ToString(),
                        Amount = (line.Total + line.TotalVAT).ToString(),
                        Tax = line.TotalVAT.ToString("0.00")
                    });
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error building print line items from invoice.");
            }

            string receiptType = printInvoice.InvoiceFlag == "02" ? "CreditNote"
                               : printInvoice.InvoiceFlag == "03" ? "DebitNote"
                               : "FiscalInvoice";

            decimal receiptTotal = 0;
            decimal.TryParse(printInvoice.InvoiceAmount,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out receiptTotal);

            decimal totalTaxAmount = printTaxes.Sum(t => t.TaxAmount);

            LocalReport localReport = reportViewer1.LocalReport;
            switch (ConfigurationManager.AppSettings.Get("PageSize"))
            {
                case "Till Slips":
                    reportViewer1.LocalReport.ReportPath = "TillSlipReport.rdlc";
                    break;
                case "A4":
                    reportViewer1.LocalReport.ReportPath = "A4Report.rdlc";
                    break;
                default:
                    reportViewer1.LocalReport.ReportPath = "TillSlipReport.rdlc";
                    break;
            }

            reportViewer1.LocalReport.DataSources.Clear();
            string basePath = ConfigurationManager.AppSettings.Get("BasePath");
            string logoDir = Path.Combine(basePath, "Logo");

            ReportDataSource reportDataSource = new ReportDataSource("DataSet1", itemlist);
            ReportDataSource reportDataSource2 = new ReportDataSource("TaxTable", taxTables);
            ReportParameter[] reportParameters = new ReportParameter[34];
            string logoFilePath = Directory.GetFiles(logoDir, "logo.*").FirstOrDefault();

            string imagePath = logoFilePath != null
                ? new Uri(logoFilePath).AbsoluteUri
                : new Uri(logoDir + $"\\{ConfigurationManager.AppSettings.Get("LogoFile")}").AbsoluteUri;

            reportParameters[0]  = new ReportParameter("ImagePath", imagePath);
            reportParameters[1]  = new ReportParameter("CompanyName", CardDetails.Data.CompanyName);
            reportParameters[2]  = new ReportParameter("Address", ConfigurationManager.AppSettings.Get("CompanyAddress"));
            reportParameters[3]  = new ReportParameter("TIN", CardDetails.Data.TIN);
            reportParameters[4]  = new ReportParameter("VatNumber", CardDetails.Data.VAT);
            reportParameters[5]  = new ReportParameter("Signature", printInvoice.VerificationCode);
            reportParameters[6]  = new ReportParameter("InvoiceNumber", printInvoice.InvoiceNumber);
            reportParameters[7]  = new ReportParameter("Tax", totalTaxAmount.ToString());
            reportParameters[8]  = new ReportParameter("Total", receiptTotal.ToString());
            reportParameters[9]  = new ReportParameter("Cashier", printInvoice.Cashier);
            reportParameters[10] = new ReportParameter("Date", DateTime.Now.ToString());
            reportParameters[11] = new ReportParameter("Currency", printInvoice.Currency);
            reportParameters[12] = new ReportParameter("ReceiptLabel",
                receiptType == "FiscalInvoice"
                    ? ReceiptLabel
                    : (ConfigurationManager.AppSettings.Get("NumberOfCopies") == "2"
                        ? $"COPY OF {receiptType}"
                        : receiptType));
            reportParameters[13] = new ReportParameter("customerName", CustomerName);
            reportParameters[14] = new ReportParameter("CustomerNum", CustomerNumber);
            reportParameters[15] = new ReportParameter("DeviceId", ConfigurationManager.AppSettings.Get("DeviceId") ?? "");
            reportParameters[16] = new ReportParameter("ReceiptGlobalNumber", "0");
            reportParameters[17] = new ReportParameter("ReceiptNumber", "0");
            reportParameters[18] = new ReportParameter("FiscalDay", "");
            reportParameters[19] = new ReportParameter("ReceiptId", "");
            reportParameters[20] = new ReportParameter("Reason", printInvoice.InvoiceFlag == "02" ? printInvoice.ReasonForCreditNote : null);
            reportParameters[21] = new ReportParameter("DeviceSerial", CardDetails.Data.SerialNumber);
            reportParameters[22] = new ReportParameter("BuyerRegisterName", !string.IsNullOrEmpty(printInvoice.CustomerName) ? printInvoice.CustomerName : null);
            reportParameters[23] = new ReportParameter("BuyerTradeName", !string.IsNullOrEmpty(printInvoice.CustomerName) ? printInvoice.CustomerName : null);
            reportParameters[24] = new ReportParameter("BuyerTIN", !string.IsNullOrEmpty(printInvoice.CustomerBPN) ? printInvoice.CustomerBPN : null);
            reportParameters[25] = new ReportParameter("BuyerVATNumber", !string.IsNullOrEmpty(printInvoice.CustomerVATNumber) ? printInvoice.CustomerVATNumber : null);
            reportParameters[26] = new ReportParameter("BuyerContacts", !string.IsNullOrEmpty(printInvoice.CustomerTelephoneNumber) ? printInvoice.CustomerTelephoneNumber : null);
            reportParameters[27] = new ReportParameter("BuyerAddress", !string.IsNullOrEmpty(printInvoice.CustomerAddress) ? printInvoice.CustomerAddress : null);
            reportParameters[28] = new ReportParameter("PhoneNumber", ConfigurationManager.AppSettings.Get("PhoneNumber"));
            reportParameters[29] = new ReportParameter("Email", ConfigurationManager.AppSettings.Get("Email"));
            reportParameters[30] = new ReportParameter("InvoiceComment", !string.IsNullOrEmpty(printInvoice.OriginalInvoiceNumber) ? printInvoice.OriginalInvoiceNumber : null);
            reportParameters[31] = new ReportParameter("VerificationLink", printInvoice.VerificationLink);
            reportParameters[32] = new ReportParameter("CustomerEmail", printInvoice.CustomerEmail);
            reportParameters[33] = new ReportParameter("Notes", _printNotes);



            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource2);

            ReportData qrReportData = new ReportData();
            if (!string.IsNullOrWhiteSpace(printInvoice.QRCode))
            {
                QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
                QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(printInvoice.QRCode, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);

                Bitmap bmp = qRCode.GetGraphic(7);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Bmp);
                    ReportData.QRCodeRow qRCodeRow = qrReportData.QRCode.NewQRCodeRow();
                    qRCodeRow.Image = ms.ToArray();
                    qrReportData.QRCode.AddQRCodeRow(qRCodeRow);
                }
            }
            // Always add ReportData source — empty table when no QR so the dataset resolves
            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource1.Name = "ReportData";
            reportDataSource1.Value = qrReportData.QRCode;
            reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            /*            PaperSize paperSize = new PaperSize("receipt", 80, 297);
                        PageSettings pageSettings = new PageSettings();
                        pageSettings.PaperSize = paperSize;
                        reportViewer1.SetPageSettings(pageSettings);*/


            // MessageBox.Show(reportViewer1.LocalReport.GetDefaultPageSettings().PaperSize.ToString()); 

            reportViewer1.RefreshReport();
            reportViewer1.LocalReport.Print();
            //printInvoice.Clear();
            _mra.ActivateTerminalAsync();
            _mra.RefreshSiteProductsAsync();
            _mra.GetTerminalSiteProductsAsync();
            _mra.GetLatestConfigsAsync();
            _mra.GetCachedLicenseJson();
           


            // printInvoice = null;
            //  DirectoryCopy(ConfigurationManager.AppSettings.Get("SourceFolder"), ConfigurationManager.AppSettings.Get("TargetFolder"));


        }








        private void btnInstall_Click()
        {
            string status = "";


            try
            {
                if (Directory.Exists(@"C:\Revmax\Revmax") == false)
                {

                    Directory.CreateDirectory(@"C:\Revmax\Revmax");

                }


               moveDirectory(new DirectoryInfo(@"RevLive"), new DirectoryInfo(@"C:\Revmax\Revmax\RevLive"));



                status = CMDStart(@"C:\REVMAX\Revmax\RevLive\RevLive.exe", "/install");

                status = CMDStart(@"C:\REVMAX\Revmax\RevLive\RevLive.exe", "/start");



                checkServices();

            }
            catch (Exception ex)
            {

            }
        }

        private void moveDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            try
            {
                if (!destination.Exists)
                {
                    destination.Create();
                }

                // Copy all files.
                FileInfo[] files = source.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(destination.FullName,
                        file.Name));
                }

                // Process subdirectories.
                DirectoryInfo[] dirs = source.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    // Get destination directory.
                    string destinationDir = Path.Combine(destination.FullName, dir.Name);

                    // Call CopyDirectory() recursively.
                    moveDirectory(dir, new DirectoryInfo(destinationDir));
                }
            }
            catch (Exception)
            {
                // TODO: Handle the exception that has been thrown
            }

        }

        private string CMDStart(string programme, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = $"{programme}";
            startInfo.Arguments = $" {arguments}";
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            return result;
        }

        private void checkServices()
        {
            ServiceController[] services = ServiceController.GetServices();
            ServiceController scLive;
            ServiceController scPortal;

            if (Array.Exists(services, element => element.ServiceName.ToString() == "RevLive"))
            {
                scLive = new ServiceController("RevLive");



            }
            CMDStart(@"C:\REVMAX\Revmax\RevLive\RevLive.exe", "/start");

        }






        private void btnLogs_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\ProgramData\RevMax\Info\Logs";
            string status = "";

            if (Directory.Exists(folderPath))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = $"explorer.exe";
                startInfo.Arguments = $" {folderPath}";
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

            }

        }












        private async Task<bool> FiscalizeAsync(Invoice invoice)
        {
            printInvoice = invoice;

            if (String.IsNullOrEmpty(invoice.CustomerBPN))
            {
                invoice.CustomerName = String.Empty;
                invoice.CustomerBPN = String.Empty;
                invoice.CustomerAddress = String.Empty;
                invoice.CustomerTelephoneNumber = String.Empty;
                invoice.CustomerVATNumber = String.Empty;
            }

            // Parse ItemsXML into line items
            var parsedItems = ParseItemsXml(invoice.ItemsXML);
            if (parsedItems.Count == 0)
            {
                MessageBox.Show("No line items could be parsed from the receipt.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string invoiceLineItemsJson;
            string levyBreakDownJson;

            if (invoice.ReceiptLines != null && invoice.ReceiptLines.Count > 0)
            {
                // Use pre-built data from Convolve (product matching already applied)
                var apiLineItems = invoice.ReceiptLines.Select((x, idx) => new
                {
                    id = idx,
                    productCode = x.ProductCode ?? ".",
                    description = x.Description ?? "",
                    unitPrice = x.UnitPrice,
                    quantity = x.Quantity,
                    discount = x.Discount,
                    total = x.Total,
                    totalVAT = x.TotalVAT,
                    taxRateId = x.TaxRateId ?? "A",
                    isProduct = x.IsProduct
                }).ToList();
                invoiceLineItemsJson = JsonConvert.SerializeObject(apiLineItems, Newtonsoft.Json.Formatting.Indented);
                levyBreakDownJson = invoice.LevyBreakDownJson ?? "[]";
            }
            else
            {
                // Fallback: build from ItemsXML with product matching (used by ReadSmallReceipt)
                bool useProductMatch = ConfigurationManager.AppSettings.Get("GetConfigProductCode") == "true";
                var lineItems = parsedItems.Select((x, idx) =>
                {
                    string productCode = x.ITEMCODE ?? ".";
                    string description = x.ITEMNAME1 ?? "";
                    bool isProduct = false;

                    if (useProductMatch && CachedSiteProducts.Count > 0)
                    {
                        var matched = ProductMatcher.FindBestMatchingProduct(description, CachedSiteProducts);
                        if (matched != null)
                        {
                            productCode = matched.ProductCode ?? productCode;
                            description = !string.IsNullOrWhiteSpace(matched.Description) ? matched.Description : description;
                            isProduct = matched.IsProduct ?? false;
                        }
                    }

                    decimal taxR = 0;
                    decimal.TryParse(x.TaxR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out taxR);
                    decimal total = 0;
                    decimal.TryParse(x.Amount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out total);
                    decimal totalVAT = 0;
                    decimal.TryParse(x.Tax, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out totalVAT);
                    decimal unitPrice = 0;
                    decimal.TryParse(x.Price, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out unitPrice);
                    decimal qty = 1;
                    decimal.TryParse(x.Quantity, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out qty);

                    return new
                    {
                        id = idx,
                        productCode,
                        description,
                        unitPrice,
                        quantity = qty,
                        discount = 0m,
                        total,
                        totalVAT,
                        taxRateId = taxR > 0 ? "A" : "E",
                        isProduct
                    };
                }).ToList();

                invoiceLineItemsJson = JsonConvert.SerializeObject(lineItems, Newtonsoft.Json.Formatting.Indented);

                var levyBreakdown = lineItems
                    .GroupBy(x => x.taxRateId)
                    .Select(g => new
                    {
                        levyTypeId = g.Key,
                        levyRate = g.Key == "A" ? (double)decimal.Parse(ConfigurationManager.AppSettings.Get("VatA") ?? "0.165") : 0.0,
                        levyAmount = (double)g.Sum(x => x.totalVAT)
                    }).ToList();
                levyBreakDownJson = JsonConvert.SerializeObject(levyBreakdown);
            }

            decimal overallTax = 0;
            decimal.TryParse(invoice.InvoiceTaxAmount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out overallTax);
            decimal overallTotal = 0;
            decimal.TryParse(invoice.InvoiceAmount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out overallTotal);

            string buyerTIN = string.IsNullOrWhiteSpace(invoice.CustomerBPN) ? null : invoice.CustomerBPN;
            string buyerName = string.IsNullOrWhiteSpace(invoice.CustomerBPN) ? null : invoice.CustomerName;

            try
            {
                string submitPreview =
                    $"posInvoiceNumber : {invoice.InvoiceNumber}\n" +
                    $"invoiceDateTime  : {DateTime.Now}\n" +
                    $"paymentMethod    : Cash\n" +
                    $"buyerTIN         : {buyerTIN ?? "(null)"}\n" +
                    $"buyerName        : {buyerName ?? "(null)"}\n" +
                    $"totalVAT         : {overallTax}\n" +
                    $"invoiceTotal     : {overallTotal}\n" +
                    $"amountTendered   : {overallTotal}\n\n" +
                    $"levyBreakDownJson:\n{levyBreakDownJson}\n\n" +
                    $"invoiceLineItemsJson:\n{invoiceLineItemsJson}";

                MessageBox.Show(submitPreview, "SubmitSaleSmartAsync — Data Preview",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                var response = await _mra.SubmitSaleSmartAsync(
                    posInvoiceNumber: invoice.InvoiceNumber,
                    invoiceDateTime: DateTime.Now,
                    invoiceLineItemsJson: invoiceLineItemsJson,
                    paymentMethod: "Cash",
                    levyBreakDownJson: levyBreakDownJson,
                    buyerTIN: buyerTIN,
                    buyerName: buyerName,
                    buyerAuthorizationCode: null,
                    totalVAT: overallTax,
                    invoiceTotal: overallTotal,
                    amountTendered: overallTotal
                );

                if (response != null && response.StatusCode == 1 && response.Data != null)
                {
                    string validationUrl = response.Data.ValidationURL ?? "";
                    string mraInvoiceNumber = invoice.InvoiceNumber;
                    decimal vatARate = 0;
                    decimal.TryParse(ConfigurationManager.AppSettings.Get("VatA") ?? "0.175",
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out vatARate);

                    // Build ReceiptLines from Convolve invoice data (already matched/resolved)
                    List<ReceiptLine> printLines;
                    if (invoice.ReceiptLines != null && invoice.ReceiptLines.Count > 0)
                    {
                        printLines = invoice.ReceiptLines.Select((x, idx) => new ReceiptLine
                        {
                            ReceiptLineName = x.Description ?? "",
                            ReceiptLineNo = idx + 1,
                            ReceiptLineQuantity = x.Quantity,
                            ReceiptLineType = "Sale",
                            ReceiptLinePrice = x.UnitPrice,
                            ReceiptLineTotal = x.Total + x.TotalVAT,
                            TaxCode = x.TaxRateId == "A" ? "A" : "B",
                            TaxPercent = x.TaxRateId == "A" ? vatARate * 100 : 0
                        }).ToList();
                    }
                    else
                    {
                        printLines = parsedItems.Select((x, idx) =>
                        {
                            decimal taxR2 = 0; decimal.TryParse(x.TaxR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out taxR2);
                            decimal total2 = 0; decimal.TryParse(x.Amount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out total2);
                            decimal price2 = 0; decimal.TryParse(x.Price, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out price2);
                            decimal qty2 = 1; decimal.TryParse(x.Quantity, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out qty2);
                            return new ReceiptLine
                            {
                                ReceiptLineName = x.ITEMNAME1 ?? "",
                                ReceiptLineNo = idx + 1,
                                ReceiptLineQuantity = qty2,
                                ReceiptLinePrice = price2,
                                ReceiptLineTotal = total2,
                                TaxCode = taxR2 > 0 ? "A" : "B",
                                TaxPercent = taxR2 * 100
                            };
                        }).ToList();
                    }

                    // Build ReceiptTaxes: prefer MRA TaxSummary, fall back to invoice lines
                    List<ReceiptTax> printTaxes;
                    if (response.Data.TaxSummary != null)
                    {
                        printTaxes = BuildReceiptTaxes(response.Data.TaxSummary, parsedItems);
                    }
                    else if (invoice.ReceiptLines != null && invoice.ReceiptLines.Count > 0)
                    {
                        printTaxes = BuildReceiptTaxesFromLines(invoice.ReceiptLines, vatARate);
                    }
                    else
                    {
                        printTaxes = BuildReceiptTaxes(null, parsedItems);
                    }

                    rootinv = new RootObject
                    {
                        Code = "1",
                        Message = response.Remark ?? "Success",
                        QRcode = validationUrl,
                        VerificationCode = validationUrl,
                        VerificationLink = validationUrl,
                        FiscalDay = "",
                        DeviceId = ConfigurationManager.AppSettings.Get("DeviceId") ?? "",
                        Data = new Data
                        {
                            Receipt = new Receipt
                            {
                                InvoiceNo = mraInvoiceNumber,
                                ReceiptDate = DateTime.Now,
                                ReceiptTotal = overallTotal,
                                ReceiptType = invoice.InvoiceFlag == "02" ? "CreditNote"
                                            : invoice.InvoiceFlag == "03" ? "DebitNote"
                                            : "FiscalInvoice",
                                ReceiptGlobalNo = 0,
                                ReceiptCounter = 0,
                                ReceiptLinesTaxInclusive = ConfigurationManager.AppSettings.Get("VatFlag") == "1",
                                ReceiptLines = printLines,
                                ReceiptTaxes = printTaxes
                            }
                        }
                    };

                    invoice.VerificationLink = validationUrl;
                    invoice.QRCode = validationUrl;
                    invoice.VerificationCode = validationUrl;
                    invoice.InvoiceNumber = mraInvoiceNumber;
                    printInvoice = invoice;

                    _printNotes = "";
                    string itype = invoice.InvoiceFlag == "01" ? "InvoiceNumber" : "CreditNoteNumber";
                    addToConfig("CurrentInvoiceNumber", $"{itype.PadRight(16)} {mraInvoiceNumber}");
                    addToConfig("Message", $"Last Fiscal Transaction: {mraInvoiceNumber} : {DateTime.Now}");
                    return true;
                }
                else
                {
                    var errorParts = new List<string>();
                    if (!string.IsNullOrEmpty(response?.Remark))
                        errorParts.Add(response.Remark);

                    var errCollection = response?.Errors as System.Collections.IEnumerable;
                    if (errCollection != null)
                    {
                        foreach (var err in errCollection)
                        {
                            string errStr = err?.ToString();
                            if (!string.IsNullOrWhiteSpace(errStr))
                                errorParts.Add(errStr);
                        }
                    }

                    string errorMsg = errorParts.Count > 0
                        ? string.Join("\n\n", errorParts)
                        : "Unknown error from MRA API";

                    _printNotes = errorMsg;
                    printInvoice.QRCode = "";
                    printInvoice.VerificationCode = "";
                    printInvoice.VerificationLink = "";
                    MessageBox.Show(errorMsg, "Fiscalisation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    addToConfig("Message", $"Last Fiscal Transaction Failed: {invoice.InvoiceNumber} : {DateTime.Now}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _printNotes = ex.Message;
                printInvoice.QRCode = "";
                printInvoice.VerificationCode = "";
                printInvoice.VerificationLink = "";
                MessageBox.Show($"Error during fiscalisation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addToConfig("Message", $"Last Fiscal Transaction Failed: {invoice.InvoiceNumber} : {DateTime.Now}");
                return false;
            }
        }

        private static List<item> ParseItemsXml(string itemsXml)
        {
            var result = new List<item>();
            if (string.IsNullOrWhiteSpace(itemsXml)) return result;
            try
            {
                var xdoc = XDocument.Parse(itemsXml);
                foreach (var el in xdoc.Descendants("ITEM"))
                {
                    result.Add(new item
                    {
                        HH = el.Element("HH")?.Value?.Trim() ?? "",
                        ITEMCODE = el.Element("ITEMCODE")?.Value?.Trim() ?? ".",
                        ITEMNAME1 = el.Element("ITEMNAME1")?.Value?.Trim() ?? "",
                        ITEMNAME2 = el.Element("ITEMNAME2")?.Value?.Trim() ?? "",
                        Quantity = el.Element("QTY")?.Value?.Trim() ?? "1",
                        Price = el.Element("PRICE")?.Value?.Trim() ?? "0",
                        Amount = el.Element("AMT")?.Value?.Trim() ?? "0",
                        Tax = el.Element("TAX")?.Value?.Trim() ?? "0",
                        TaxR = el.Element("TAXR")?.Value?.Trim() ?? "0"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ParseItemsXml error: {ex.Message}");
            }
            return result;
        }

        private static List<ReceiptTax> BuildReceiptTaxes(dynamic taxSummary, List<item> parsedItems)
        {
            var result = new List<ReceiptTax>();
            try
            {
                if (taxSummary != null)
                {
                    foreach (var t in taxSummary)
                    {
                        string code = t.TaxCode?.ToString() ?? "A";
                        decimal percent = 0;
                        decimal.TryParse(t.VatPercent?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out percent);
                        decimal amount = 0;
                        decimal.TryParse(t.Amount?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amount);
                        decimal taxableAmount = 0;
                        decimal.TryParse(t.TaxableAmount?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out taxableAmount);
                        // Map MRA codes to legacy codes expected by Print(): A=standard, E/B=zero-rated→"B"
                        string legacyCode = (code == "A") ? "A" : "B";
                        result.Add(new ReceiptTax
                        {
                            TaxCode = legacyCode,
                            TaxPercent = percent,
                            TaxAmount = amount,
                            SalesAmountWithTax = taxableAmount
                        });
                    }
                    return result;
                }
            }
            catch { }

            // Fallback: compute from parsed items
            var grouped = parsedItems.GroupBy(x =>
            {
                decimal r = 0;
                decimal.TryParse(x.TaxR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out r);
                return r > 0 ? "A" : "B";
            });
            foreach (var g in grouped)
            {
                decimal totalAmt = 0, totalTax = 0, taxRate = 0;
                foreach (var x in g)
                {
                    decimal amt = 0; decimal.TryParse(x.Amount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amt);
                    decimal tx = 0; decimal.TryParse(x.Tax, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tx);
                    decimal r = 0; decimal.TryParse(x.TaxR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out r);
                    totalAmt += amt; totalTax += tx; if (r > taxRate) taxRate = r;
                }
                result.Add(new ReceiptTax
                {
                    TaxCode = g.Key,
                    TaxPercent = taxRate * 100,
                    TaxAmount = totalTax,
                    SalesAmountWithTax = totalAmt
                });
            }
            return result;
        }

        private static List<ReceiptTax> BuildReceiptTaxesFromLines(List<MraReceiptLine> lines, decimal vatARate)
        {
            var result = new List<ReceiptTax>();
            var grouped = lines.GroupBy(x => x.TaxRateId == "A" ? "A" : "B");
            foreach (var g in grouped)
            {
                decimal totalBase = g.Sum(x => x.Total);
                decimal totalTax = g.Sum(x => x.TotalVAT);
                result.Add(new ReceiptTax
                {
                    TaxCode = g.Key,
                    TaxPercent = g.Key == "A" ? vatARate * 100 : 0,
                    TaxAmount = totalTax,
                    SalesAmountWithTax = totalBase + totalTax
                });
            }
            return result;
        }


        public void addToConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void Form1_Load(object sender, EventArgs e)

        {

            this.reportViewer1.RefreshReport();
 
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }

        private void moveFile()
        {

            try
            {
                string sourcePath = ConfigurationManager.AppSettings.Get("SourceFolder");
                string targetPath = ConfigurationManager.AppSettings.Get("TargetFolder");
                string fileName = string.Empty;
                string destFile = string.Empty;


                if (System.IO.Directory.Exists(sourcePath))
                {
                    string[] files = System.IO.Directory.GetFiles(sourcePath);

                    // Copy the files and overwrite destination files if they already exist. 
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
                else
                {
                    Console.WriteLine("Source path does not exist!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving file" + ex.Message);
            }


        }

        private async void zReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _mra.RefreshSiteProductsAsync();
                string json = JsonConvert.SerializeObject(result, Formatting.Indented);
                SaveAndOpenResponse(json, "RefreshSiteProducts");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Refresh Site Products Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _mra.GetTerminalSiteProductsAsync();
                string json = JsonConvert.SerializeObject(result, Formatting.Indented);
                SaveAndOpenResponse(json, "GetSiteProducts");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Get Site Products Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ZReading()
        {
            // ZReport not supported with MRA — no-op
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void checkLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _mra.ActivateTerminalAsync();
                string json = JsonConvert.SerializeObject(result, Formatting.Indented);
                SaveAndOpenResponse(json, "ActivateTerminal");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Activate Terminal Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void setLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await _mra.GetLatestConfigsAsync();
                string json = JsonConvert.SerializeObject(result, Formatting.Indented);
                SaveAndOpenResponse(json, "GetLatestConfigs");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Get Latest Configs Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void retrainInterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string json = _mra.GetCachedLicenseJson();
                if (string.IsNullOrEmpty(json))
                {
                    MessageBox.Show("No cached license available.", "Get License", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveAndOpenResponse(json, "GetLicense");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Get License Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SaveAndOpenResponse(string json, string label)
        {
            string preview = json.Length > 300 ? json.Substring(0, 300) + "\n\n..." : json;
            var choice = MessageBox.Show(
                $"{preview}\n\nSave and open full response?",
                label, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (choice != DialogResult.Yes) return;

            string folder = @"C:\responses";
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"{label}.json");
            File.WriteAllText(filePath, json, Encoding.UTF8);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show($"Company:  {CardDetails.Data.CompanyName}");
            Configurations configurations = new Configurations(CardDetails.Data.CompanyName);
            configurations.ShowDialog();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        

        private void timer2_Tick(object sender, EventArgs e)
        {
            // AutomateZReport not supported with MRA fiscal system
        }




        private async Task ProcessSampleInvoicesAsync()
        {
            MessageBox.Show("Sample invoice generation is not supported with the MRA fiscal system.", "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await Task.CompletedTask;
        }

        private async Task TESTZREPORT()
        {
            // ZReport not supported with MRA fiscal system
            await Task.CompletedTask;
        }

        private void generateInvoicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PasswordForm passwordForm = new PasswordForm())
            {
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    string enteredPassword = passwordForm.Password;
                    // Replace "yourCorrectPassword" with the actual password you want to check
                    if (enteredPassword == ConfigurationManager.AppSettings.Get("RandomNumber"))
                    {
                        // Proceed with the save operation

                        if (MessageBox.Show("Are You Sure you want To generate Sample Invoices, Make sure All is set:\n**Printer Microsoft Print To PDF**\n**Logo File Configured**\n**Device configured in Test with Certifiicates**\n**License Keys set**", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ProcessSampleInvoicesAsync();

                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password. You can't proceed to Generate Sample templates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void contextMenuStrip1_DoubleClick(object sender, EventArgs e)
        {
            generateInvoicesToolStripMenuItem.Visible = true;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            generateInvoicesToolStripMenuItem.Visible = true;
        }
    }
}

