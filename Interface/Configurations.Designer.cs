namespace Revmax_Interface_Promun
{
    partial class Configurations
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configurations));
            this.confirm = new System.Windows.Forms.Button();
            this.heading = new System.Windows.Forms.Label();
            this.numberLabel = new System.Windows.Forms.Label();
            this.emailLabel = new System.Windows.Forms.Label();
            this.pageSizeLabel = new System.Windows.Forms.Label();
            this.basePathLabel = new System.Windows.Forms.Label();
            this.phoneNumber = new System.Windows.Forms.TextBox();
            this.email = new System.Windows.Forms.TextBox();
            this.pageSize = new System.Windows.Forms.ComboBox();
            this.folderPath = new System.Windows.Forms.Label();
            this.choosePath = new System.Windows.Forms.Button();
            this.folderPathText = new System.Windows.Forms.TextBox();
            this.creditNote = new System.Windows.Forms.CheckBox();
            this.debitNote = new System.Windows.Forms.CheckBox();
            this.invoice = new System.Windows.Forms.CheckBox();
            this.invoiceType = new System.Windows.Forms.Label();
            this.printNameKeyOLD = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.companyAddress = new System.Windows.Forms.RichTextBox();
            this.receiptPathValue = new System.Windows.Forms.TextBox();
            this.chooseReceiptPath = new System.Windows.Forms.Button();
            this.receiptPathLabel = new System.Windows.Forms.Label();
            this.receiptPath = new System.Windows.Forms.Label();
            this.customPath = new System.Windows.Forms.CheckBox();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.printerNameText = new System.Windows.Forms.TextBox();
            this.printerNameLabel = new System.Windows.Forms.Label();
            this.pointPrinter = new System.Windows.Forms.CheckBox();
            this.printNameKey = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numCopies = new System.Windows.Forms.NumericUpDown();
            this.autoZReport = new System.Windows.Forms.CheckBox();
            this.myMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // confirm
            // 
            this.confirm.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.confirm.ForeColor = System.Drawing.Color.MidnightBlue;
            this.confirm.Location = new System.Drawing.Point(134, 472);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(420, 32);
            this.confirm.TabIndex = 0;
            this.confirm.Text = "Confirm Changes";
            this.confirm.UseVisualStyleBackColor = true;
            this.confirm.Click += new System.EventHandler(this.confirm_Click);
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heading.ForeColor = System.Drawing.Color.MidnightBlue;
            this.heading.Location = new System.Drawing.Point(16, 9);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(322, 24);
            this.heading.TabIndex = 1;
            this.heading.Text = "CompanyName Configurations";
            this.heading.Click += new System.EventHandler(this.heading_Click);
            // 
            // numberLabel
            // 
            this.numberLabel.AutoSize = true;
            this.numberLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberLabel.Location = new System.Drawing.Point(12, 111);
            this.numberLabel.Name = "numberLabel";
            this.numberLabel.Size = new System.Drawing.Size(126, 19);
            this.numberLabel.TabIndex = 2;
            this.numberLabel.Text = "Phone Number:";
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailLabel.Location = new System.Drawing.Point(12, 146);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(63, 19);
            this.emailLabel.TabIndex = 3;
            this.emailLabel.Text = "Email:";
            // 
            // pageSizeLabel
            // 
            this.pageSizeLabel.AutoSize = true;
            this.pageSizeLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageSizeLabel.Location = new System.Drawing.Point(12, 184);
            this.pageSizeLabel.Name = "pageSizeLabel";
            this.pageSizeLabel.Size = new System.Drawing.Size(108, 19);
            this.pageSizeLabel.TabIndex = 4;
            this.pageSizeLabel.Text = "Paper Size:";
            // 
            // basePathLabel
            // 
            this.basePathLabel.AutoSize = true;
            this.basePathLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.basePathLabel.Location = new System.Drawing.Point(12, 290);
            this.basePathLabel.Name = "basePathLabel";
            this.basePathLabel.Size = new System.Drawing.Size(99, 19);
            this.basePathLabel.TabIndex = 5;
            this.basePathLabel.Text = "Base Path:";
            // 
            // phoneNumber
            // 
            this.phoneNumber.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phoneNumber.Location = new System.Drawing.Point(224, 114);
            this.phoneNumber.Name = "phoneNumber";
            this.phoneNumber.Size = new System.Drawing.Size(328, 26);
            this.phoneNumber.TabIndex = 6;
            // 
            // email
            // 
            this.email.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.email.Location = new System.Drawing.Point(224, 146);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(328, 26);
            this.email.TabIndex = 7;
            this.email.TextChanged += new System.EventHandler(this.email_TextChanged);
            // 
            // pageSize
            // 
            this.pageSize.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageSize.FormattingEnabled = true;
            this.pageSize.Location = new System.Drawing.Point(224, 181);
            this.pageSize.Name = "pageSize";
            this.pageSize.Size = new System.Drawing.Size(328, 27);
            this.pageSize.TabIndex = 8;
            this.pageSize.Text = "Select Paper Size";
            this.pageSize.SelectedIndexChanged += new System.EventHandler(this.pageSize_SelectedIndexChanged);
            // 
            // folderPath
            // 
            this.folderPath.AutoSize = true;
            this.folderPath.Font = new System.Drawing.Font("Consolas", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.folderPath.Location = new System.Drawing.Point(221, 278);
            this.folderPath.Name = "folderPath";
            this.folderPath.Size = new System.Drawing.Size(67, 13);
            this.folderPath.TabIndex = 9;
            this.folderPath.Text = "folderPath";
            // 
            // choosePath
            // 
            this.choosePath.BackColor = System.Drawing.Color.MidnightBlue;
            this.choosePath.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.choosePath.ForeColor = System.Drawing.Color.Snow;
            this.choosePath.Location = new System.Drawing.Point(134, 290);
            this.choosePath.Name = "choosePath";
            this.choosePath.Size = new System.Drawing.Size(75, 23);
            this.choosePath.TabIndex = 10;
            this.choosePath.Text = "Choose";
            this.choosePath.UseVisualStyleBackColor = false;
            this.choosePath.Click += new System.EventHandler(this.choosePath_Click);
            // 
            // folderPathText
            // 
            this.folderPathText.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.folderPathText.Location = new System.Drawing.Point(224, 290);
            this.folderPathText.Name = "folderPathText";
            this.folderPathText.Size = new System.Drawing.Size(328, 26);
            this.folderPathText.TabIndex = 11;
            this.folderPathText.TextChanged += new System.EventHandler(this.folderPathText_TextChanged);
            // 
            // creditNote
            // 
            this.creditNote.AutoSize = true;
            this.creditNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.creditNote.ForeColor = System.Drawing.Color.IndianRed;
            this.creditNote.Location = new System.Drawing.Point(581, 146);
            this.creditNote.Name = "creditNote";
            this.creditNote.Size = new System.Drawing.Size(150, 29);
            this.creditNote.TabIndex = 12;
            this.creditNote.Text = "Credit Note";
            this.creditNote.UseVisualStyleBackColor = true;
            this.creditNote.Visible = false;
            this.creditNote.CheckedChanged += new System.EventHandler(this.creditNote_CheckedChanged);
            // 
            // debitNote
            // 
            this.debitNote.AutoSize = true;
            this.debitNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.debitNote.ForeColor = System.Drawing.Color.MidnightBlue;
            this.debitNote.Location = new System.Drawing.Point(581, 178);
            this.debitNote.Name = "debitNote";
            this.debitNote.Size = new System.Drawing.Size(142, 29);
            this.debitNote.TabIndex = 13;
            this.debitNote.Text = "Debit Note";
            this.debitNote.UseVisualStyleBackColor = true;
            this.debitNote.Visible = false;
            this.debitNote.CheckedChanged += new System.EventHandler(this.debitNote_CheckedChanged);
            // 
            // invoice
            // 
            this.invoice.AutoSize = true;
            this.invoice.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invoice.ForeColor = System.Drawing.Color.LimeGreen;
            this.invoice.Location = new System.Drawing.Point(581, 111);
            this.invoice.Name = "invoice";
            this.invoice.Size = new System.Drawing.Size(106, 29);
            this.invoice.TabIndex = 14;
            this.invoice.Text = "Invoice";
            this.invoice.UseVisualStyleBackColor = true;
            this.invoice.CheckedChanged += new System.EventHandler(this.invoice_CheckedChanged);
            // 
            // invoiceType
            // 
            this.invoiceType.AutoSize = true;
            this.invoiceType.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invoiceType.Location = new System.Drawing.Point(320, 70);
            this.invoiceType.Name = "invoiceType";
            this.invoiceType.Size = new System.Drawing.Size(130, 24);
            this.invoiceType.TabIndex = 15;
            this.invoiceType.Text = "Invoice Type";
            this.invoiceType.Click += new System.EventHandler(this.invoiceType_Click);
            // 
            // printNameKeyOLD
            // 
            this.printNameKeyOLD.AutoSize = true;
            this.printNameKeyOLD.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printNameKeyOLD.Location = new System.Drawing.Point(649, 14);
            this.printNameKeyOLD.Name = "printNameKeyOLD";
            this.printNameKeyOLD.Size = new System.Drawing.Size(153, 19);
            this.printNameKeyOLD.TabIndex = 17;
            this.printNameKeyOLD.Text = "Current Printer:";
            this.printNameKeyOLD.Visible = false;
            this.printNameKeyOLD.Click += new System.EventHandler(this.printNameKeyOLD_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 19);
            this.label1.TabIndex = 18;
            this.label1.Text = "Company Address:";
            // 
            // companyAddress
            // 
            this.companyAddress.Location = new System.Drawing.Point(224, 214);
            this.companyAddress.Name = "companyAddress";
            this.companyAddress.Size = new System.Drawing.Size(328, 61);
            this.companyAddress.TabIndex = 19;
            this.companyAddress.Text = "";
            // 
            // receiptPathValue
            // 
            this.receiptPathValue.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receiptPathValue.Location = new System.Drawing.Point(226, 339);
            this.receiptPathValue.Name = "receiptPathValue";
            this.receiptPathValue.Size = new System.Drawing.Size(328, 26);
            this.receiptPathValue.TabIndex = 23;
            this.receiptPathValue.TextChanged += new System.EventHandler(this.receiptPathValue_TextChanged);
            // 
            // chooseReceiptPath
            // 
            this.chooseReceiptPath.BackColor = System.Drawing.Color.MidnightBlue;
            this.chooseReceiptPath.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chooseReceiptPath.ForeColor = System.Drawing.Color.Snow;
            this.chooseReceiptPath.Location = new System.Drawing.Point(134, 339);
            this.chooseReceiptPath.Name = "chooseReceiptPath";
            this.chooseReceiptPath.Size = new System.Drawing.Size(75, 23);
            this.chooseReceiptPath.TabIndex = 22;
            this.chooseReceiptPath.Text = "Choose";
            this.chooseReceiptPath.UseVisualStyleBackColor = false;
            this.chooseReceiptPath.Click += new System.EventHandler(this.chooseReceiptPath_Click);
            // 
            // receiptPathLabel
            // 
            this.receiptPathLabel.AutoSize = true;
            this.receiptPathLabel.Font = new System.Drawing.Font("Consolas", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receiptPathLabel.Location = new System.Drawing.Point(223, 327);
            this.receiptPathLabel.Name = "receiptPathLabel";
            this.receiptPathLabel.Size = new System.Drawing.Size(79, 13);
            this.receiptPathLabel.TabIndex = 21;
            this.receiptPathLabel.Text = "receiptPathy";
            // 
            // receiptPath
            // 
            this.receiptPath.AutoSize = true;
            this.receiptPath.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receiptPath.Location = new System.Drawing.Point(14, 339);
            this.receiptPath.Name = "receiptPath";
            this.receiptPath.Size = new System.Drawing.Size(126, 19);
            this.receiptPath.TabIndex = 20;
            this.receiptPath.Text = "Receipt Path:";
            // 
            // customPath
            // 
            this.customPath.AutoSize = true;
            this.customPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.customPath.Location = new System.Drawing.Point(560, 345);
            this.customPath.Name = "customPath";
            this.customPath.Size = new System.Drawing.Size(153, 20);
            this.customPath.TabIndex = 24;
            this.customPath.Text = "Point To This Path";
            this.customPath.UseVisualStyleBackColor = true;
            this.customPath.CheckedChanged += new System.EventHandler(this.customPath_CheckedChanged);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.MidnightBlue;
            this.button1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Snow;
            this.button1.Location = new System.Drawing.Point(134, 386);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 25;
            this.button1.Text = "Choose";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // printerNameText
            // 
            this.printerNameText.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printerNameText.Location = new System.Drawing.Point(226, 383);
            this.printerNameText.Name = "printerNameText";
            this.printerNameText.Size = new System.Drawing.Size(328, 26);
            this.printerNameText.TabIndex = 26;
            // 
            // printerNameLabel
            // 
            this.printerNameLabel.AutoSize = true;
            this.printerNameLabel.Font = new System.Drawing.Font("Consolas", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printerNameLabel.Location = new System.Drawing.Point(256, 367);
            this.printerNameLabel.Name = "printerNameLabel";
            this.printerNameLabel.Size = new System.Drawing.Size(73, 13);
            this.printerNameLabel.TabIndex = 27;
            this.printerNameLabel.Text = "printerName";
            // 
            // pointPrinter
            // 
            this.pointPrinter.AutoSize = true;
            this.pointPrinter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointPrinter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.pointPrinter.Location = new System.Drawing.Point(560, 389);
            this.pointPrinter.Name = "pointPrinter";
            this.pointPrinter.Size = new System.Drawing.Size(167, 20);
            this.pointPrinter.TabIndex = 28;
            this.pointPrinter.Text = "Point To This Printer";
            this.pointPrinter.UseVisualStyleBackColor = true;
            // 
            // printNameKey
            // 
            this.printNameKey.AutoSize = true;
            this.printNameKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printNameKey.Location = new System.Drawing.Point(15, 392);
            this.printNameKey.Name = "printNameKey";
            this.printNameKey.Size = new System.Drawing.Size(118, 20);
            this.printNameKey.TabIndex = 29;
            this.printNameKey.Text = "Printer Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 423);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 20);
            this.label2.TabIndex = 30;
            this.label2.Text = "Number Of Copies:";
            // 
            // numCopies
            // 
            this.numCopies.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCopies.Location = new System.Drawing.Point(226, 421);
            this.numCopies.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCopies.Name = "numCopies";
            this.numCopies.Size = new System.Drawing.Size(326, 26);
            this.numCopies.TabIndex = 31;
            this.numCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // autoZReport
            // 
            this.autoZReport.AutoSize = true;
            this.autoZReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoZReport.ForeColor = System.Drawing.Color.LimeGreen;
            this.autoZReport.Location = new System.Drawing.Point(581, 236);
            this.autoZReport.Name = "autoZReport";
            this.autoZReport.Size = new System.Drawing.Size(221, 29);
            this.autoZReport.TabIndex = 32;
            this.autoZReport.Text = "Automate ZReport";
            this.autoZReport.UseVisualStyleBackColor = true;
            this.autoZReport.CheckedChanged += new System.EventHandler(this.autoZReport_CheckedChanged);
            // 
            // myMessage
            // 
            this.myMessage.AutoSize = true;
            this.myMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myMessage.ForeColor = System.Drawing.Color.Red;
            this.myMessage.Location = new System.Drawing.Point(17, 45);
            this.myMessage.Name = "myMessage";
            this.myMessage.Size = new System.Drawing.Size(47, 13);
            this.myMessage.TabIndex = 33;
            this.myMessage.Text = "..........";
            // 
            // Configurations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 556);
            this.Controls.Add(this.myMessage);
            this.Controls.Add(this.autoZReport);
            this.Controls.Add(this.numCopies);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.printNameKey);
            this.Controls.Add(this.pointPrinter);
            this.Controls.Add(this.printerNameLabel);
            this.Controls.Add(this.printerNameText);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.customPath);
            this.Controls.Add(this.receiptPathValue);
            this.Controls.Add(this.chooseReceiptPath);
            this.Controls.Add(this.receiptPathLabel);
            this.Controls.Add(this.receiptPath);
            this.Controls.Add(this.companyAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.printNameKeyOLD);
            this.Controls.Add(this.invoiceType);
            this.Controls.Add(this.invoice);
            this.Controls.Add(this.debitNote);
            this.Controls.Add(this.creditNote);
            this.Controls.Add(this.folderPathText);
            this.Controls.Add(this.choosePath);
            this.Controls.Add(this.folderPath);
            this.Controls.Add(this.pageSize);
            this.Controls.Add(this.email);
            this.Controls.Add(this.phoneNumber);
            this.Controls.Add(this.basePathLabel);
            this.Controls.Add(this.pageSizeLabel);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.numberLabel);
            this.Controls.Add(this.heading);
            this.Controls.Add(this.confirm);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Configurations";
            this.Text = "Configurations";
            this.Load += new System.EventHandler(this.Configurations_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numCopies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button confirm;
        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Label numberLabel;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label pageSizeLabel;
        private System.Windows.Forms.Label basePathLabel;
        private System.Windows.Forms.TextBox phoneNumber;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.ComboBox pageSize;
        private System.Windows.Forms.Label folderPath;
        private System.Windows.Forms.Button choosePath;
        private System.Windows.Forms.TextBox folderPathText;
        private System.Windows.Forms.CheckBox creditNote;
        private System.Windows.Forms.CheckBox debitNote;
        private System.Windows.Forms.CheckBox invoice;
        private System.Windows.Forms.Label invoiceType;
        private System.Windows.Forms.Label printNameKeyOLD;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox companyAddress;
        private System.Windows.Forms.TextBox receiptPathValue;
        private System.Windows.Forms.Button chooseReceiptPath;
        private System.Windows.Forms.Label receiptPathLabel;
        private System.Windows.Forms.Label receiptPath;
        private System.Windows.Forms.CheckBox customPath;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox printerNameText;
        private System.Windows.Forms.Label printerNameLabel;
        private System.Windows.Forms.CheckBox pointPrinter;
        private System.Windows.Forms.Label printNameKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numCopies;
        private System.Windows.Forms.CheckBox autoZReport;
        private System.Windows.Forms.Label myMessage;
    }
}