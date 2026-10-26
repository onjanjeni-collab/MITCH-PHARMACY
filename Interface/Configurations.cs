using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Revmax_Interface_Promun
{
    public partial class Configurations : Form
    {
        private readonly string Company;
        public Configurations(string companyName)
        {
            InitializeComponent();
            this.Company = companyName;
            heading.Text = Company + " Configurations";
            //MessageBox.Show("Tapoinda1");
            pageSize.Items.AddRange(new string[] { "Till Slips", "A4" });
            phoneNumber.Text= ConfigurationManager.AppSettings.Get("PhoneNumber").Replace("Cell:", "");
            email.Text= ConfigurationManager.AppSettings.Get("Email").Replace("Email:", "");
            folderPathText.Text= ConfigurationManager.AppSettings.Get("BasePath");
            folderPath.Text= ConfigurationManager.AppSettings.Get("BasePath");
            pageSize.SelectedItem = ConfigurationManager.AppSettings.Get("PageSize");
            companyAddress.Text = ConfigurationManager.AppSettings.Get("CompanyAddress");

            receiptPathLabel.Text = ConfigurationManager.AppSettings.Get("ReceiptFolder");
            receiptPathValue.Text = ConfigurationManager.AppSettings.Get("ReceiptFolder"); 
            string checkk = ConfigurationManager.AppSettings.Get("UseCustomReceiptFolder");

            string checkkprint = ConfigurationManager.AppSettings.Get("setPrinter");
            string autoZ = ConfigurationManager.AppSettings.Get("AutomateZReport");
            string printer = ConfigurationManager.AppSettings.Get("PrinterName");
            pointPrinter.Checked= checkkprint.Equals("true")? true: false;
            autoZReport.Checked= autoZ.Equals("true")? true: false;
            printerNameLabel.Text= checkkprint.Equals("true")? printer : "Default";
            printerNameText.Text= checkkprint.Equals("true")? printer : "Default";
            numCopies.Value = int.Parse(ConfigurationManager.AppSettings.Get("NumberOfCopies"));
            myMessage.Text = ConfigurationManager.AppSettings.Get("Message");

            customPath.Checked= checkk.Equals("true")? true: false;
            //MessageBox.Show("Tapoinda");
            switch (ConfigurationManager.AppSettings.Get("InvoiceFlag"))
            {
                case "01":
                    creditNote.Checked = false;
                    debitNote.Checked = false;
                    invoice.Checked = true;
                    break;

                default:
                    break;
            }
            
            printNameKeyOLD.Visible= false;

        }

        private void choosePath_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                folderPath.Text = folderBrowserDialog.SelectedPath;
                folderPathText.Text = folderBrowserDialog.SelectedPath;
                string basePath = folderBrowserDialog.SelectedPath;

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }
                Directory.CreateDirectory($"{basePath}\\Receipts");
                Directory.CreateDirectory($"{basePath}\\Backup");
                addToConfig("BasePath", folderBrowserDialog.SelectedPath);

                // Check if Logo folder exists
                string logoPath = Path.Combine(basePath, "Logo");
                string backupPath = Path.Combine(basePath, "Backup");

                if (!Directory.Exists(logoPath) || Directory.GetFiles(logoPath).Length == 0)
                {
                    DialogResult logoConfirmation = MessageBox.Show("Do you want to upload a logo file to the Logo folder?", "Logo Upload Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (logoConfirmation == DialogResult.Yes)
                    {
                        // Open file dialog for logo upload
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Image Files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
                        openFileDialog.Title = "Select Logo File";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Get the selected logo file path
                            string logoFilePath = openFileDialog.FileName;

                            // Create the Logo folder if it doesn't exist
                            if (!Directory.Exists(logoPath))
                            {
                                Directory.CreateDirectory(logoPath);
                            }

                            // Copy the logo file to the Logo folder with the name logo.png or logo.jpeg
                            string targetLogoPath = Path.Combine(logoPath, "logo" + Path.GetExtension(logoFilePath));
                            File.Copy(logoFilePath, targetLogoPath);

                            // Optional: Create a backup of the original logo
                            if (!Directory.Exists(backupPath))
                            {
                                Directory.CreateDirectory(backupPath);
                            }

                            string backupLogoPath = Path.Combine(backupPath, $"backup_logo_{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(logoFilePath)}");
                            File.Copy(logoFilePath, backupLogoPath);

                            MessageBox.Show("Logo uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory($"{basePath}\\Backup");
                }

            }
        }


        private void confirm_Click(object sender, EventArgs e)
        {
            if (debitNote.Checked || creditNote.Checked)
            {
                if (Directory.Exists(folderPathText.Text))
                {
                    addToConfig("PageSize", String.Empty);
                    addToConfig("PageSize", (string)pageSize.SelectedItem);

                    addToConfig("PhoneNumber", String.Empty);
                    addToConfig("PhoneNumber", $"Cell: {phoneNumber.Text}");

                    addToConfig("Email", String.Empty);
                    addToConfig("Email", $"Email: {email.Text}");

                    addToConfig("BasePath", String.Empty);
                    addToConfig("BasePath", folderPathText.Text);

                    addToConfig("CreditDebitReason", String.Empty);


                    addToConfig("CompanyAddress", String.Empty);
                    addToConfig("CompanyAddress", companyAddress.Text);

                    addToConfig("ReceiptFolder", String.Empty);
                    addToConfig("ReceiptFolder", receiptPathValue.Text);

                    if (customPath.Checked)
                    {
                        addToConfig("UseCustomReceiptFolder", String.Empty);
                        addToConfig("UseCustomReceiptFolder", "true");
                    }
                    else
                    {
                        addToConfig("UseCustomReceiptFolder", String.Empty);
                        addToConfig("UseCustomReceiptFolder", "false");
                    }
                    if (autoZReport.Checked)
                    {
                        addToConfig("AutomateZReport", String.Empty);
                        addToConfig("AutomateZReport", "true");
                    }
                    else
                    {
                        addToConfig("AutomateZReport", String.Empty);
                        addToConfig("AutomateZReport", "false");
                    }
                }
                else
                {
                    MessageBox.Show("Sorry The Folder Does Not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (Directory.Exists(folderPathText.Text))
                {
                    addToConfig("PageSize", (string)pageSize.SelectedItem);
                    addToConfig("PhoneNumber", $"Cell:{phoneNumber.Text}");
                    addToConfig("Email", $"Email:{email.Text}");
                    addToConfig("BasePath", folderPathText.Text);
                    addToConfig("CreditDebitReason", "INVOICE");
                    addToConfig("CompanyAddress", String.Empty);
                    addToConfig("CompanyAddress", companyAddress.Text);


                    addToConfig("PrinterName", pointPrinter.Checked? printerNameText.Text: "Default");
                    addToConfig("setPrinter", pointPrinter.Checked ? "true" : "false");
                    addToConfig("NumberOfCopies", numCopies.Value.ToString());

                    if (autoZReport.Checked)
                    {
                        addToConfig("AutomateZReport", String.Empty);
                        addToConfig("AutomateZReport", "true");
                    }
                    else
                    {
                        addToConfig("AutomateZReport", String.Empty);
                        addToConfig("AutomateZReport", "false");
                    }

                }
                else
                {
                    MessageBox.Show("Sorry The Folder Does Not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if(MessageBox.Show("Changes Applied Successfully. Would you like to Close?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                return;
            }
            
        }

        public void addToConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void folderPathText_TextChanged(object sender, EventArgs e)
        {
            folderPath.Text= folderPathText.Text.Trim();
        }

        private void invoice_CheckedChanged(object sender, EventArgs e)
        {
            if (invoice.Checked)
            {
                creditNote.Checked = false;
                debitNote.Checked = false;
                addToConfig("InvoiceFlag", "01");
                invoiceType.Text = "Normal Invoice";
                invoiceType.ForeColor = Color.Green;
               
                printNameKeyOLD.Visible= false;
            }
        }

        private void creditNote_CheckedChanged(object sender, EventArgs e)
        {
            if (creditNote.Checked)
            {
                invoice.Checked = false;
                debitNote.Checked = false;
                addToConfig("InvoiceFlag", "02");
                invoiceType.Text = creditNote.Text;
                invoiceType.ForeColor = Color.Red;
                
                printNameKeyOLD.Visible= creditNote.Checked;
            }
        }

        private void debitNote_CheckedChanged(object sender, EventArgs e)
        {
            if (debitNote.Checked)
            {
                creditNote.Checked = false;
                invoice.Checked = false;
                addToConfig("InvoiceFlag", "03");
                invoiceType.Text = debitNote.Text;
                invoiceType.ForeColor = Color.Gray;
                
                printNameKeyOLD.Visible= debitNote.Checked;
            }
        }

        private void invoiceType_Click(object sender, EventArgs e)
        {

        }

        private void Configurations_Load(object sender, EventArgs e)
        {

        }

        private void heading_Click(object sender, EventArgs e)
        {

        }

        private void pageSize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void email_TextChanged(object sender, EventArgs e)
        {

        }

        private void chooseReceiptPath_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                receiptPathLabel.Text = folderBrowserDialog.SelectedPath;
                receiptPathValue.Text = folderBrowserDialog.SelectedPath;
                string receiptPathy = folderBrowserDialog.SelectedPath;

                if (!Directory.Exists(receiptPathy))
                {
                    Directory.CreateDirectory(receiptPathy);
                }
                
                addToConfig("ReceiptFolder", folderBrowserDialog.SelectedPath);

            }
        }

        private void receiptPathValue_TextChanged(object sender, EventArgs e)
        {
            receiptPathLabel.Text = receiptPathValue.Text.Trim();
        }

        private void customPath_CheckedChanged(object sender, EventArgs e)
        {
            if (customPath.Checked)
            {
                addToConfig("UseCustomReceiptFolder", String.Empty);
                addToConfig("UseCustomReceiptFolder", "true");
            }
            else
            {
                addToConfig("UseCustomReceiptFolder", String.Empty);
                addToConfig("UseCustomReceiptFolder", "false");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Show the print dialog
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the selected printer
                PrinterSettings printerSettings = printDialog1.PrinterSettings;
                printerNameText.Text = printerSettings.PrinterName;
                printerNameLabel.Text = printerSettings.PrinterName;
                // Display the selected printer's name
                // MessageBox.Show("Selected Printer: " + printerSettings.PrinterName);
            }
            else
            {
                return;
            }
        }

        private void autoZReport_CheckedChanged(object sender, EventArgs e)
        {
            if (autoZReport.Checked)
            {
                addToConfig("AutomateZReport", String.Empty);
                addToConfig("AutomateZReport", "true");
            }
            else
            {
                addToConfig("AutomateZReport", String.Empty);
                addToConfig("AutomateZReport", "false");
            }
        }

        private void printNameKeyOLD_Click(object sender, EventArgs e)
        {

        }
    }
}
