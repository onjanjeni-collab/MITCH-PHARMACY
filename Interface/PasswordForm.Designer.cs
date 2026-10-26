namespace Revmax_Interface_Promun
{
    partial class PasswordForm
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
            this.passcode = new System.Windows.Forms.TextBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.ConfirmPass = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkPass = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // passcode
            // 
            this.passcode.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passcode.Location = new System.Drawing.Point(306, 85);
            this.passcode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.passcode.Name = "passcode";
            this.passcode.PasswordChar = '*';
            this.passcode.Size = new System.Drawing.Size(394, 45);
            this.passcode.TabIndex = 9;
            this.passcode.TextChanged += new System.EventHandler(this.email_TextChanged);
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailLabel.Location = new System.Drawing.Point(24, 90);
            this.emailLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(269, 37);
            this.emailLabel.TabIndex = 8;
            this.emailLabel.Text = "Enter Password";
            // 
            // ConfirmPass
            // 
            this.ConfirmPass.BackColor = System.Drawing.Color.LimeGreen;
            this.ConfirmPass.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConfirmPass.ForeColor = System.Drawing.Color.GhostWhite;
            this.ConfirmPass.Location = new System.Drawing.Point(254, 171);
            this.ConfirmPass.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ConfirmPass.Name = "ConfirmPass";
            this.ConfirmPass.Size = new System.Drawing.Size(266, 62);
            this.ConfirmPass.TabIndex = 28;
            this.ConfirmPass.Text = "Confirm Password";
            this.ConfirmPass.UseVisualStyleBackColor = false;
            this.ConfirmPass.Click += new System.EventHandler(this.ConfirmPass_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(222, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(302, 37);
            this.label1.TabIndex = 29;
            this.label1.Text = "Enter Old Password";
            // 
            // checkPass
            // 
            this.checkPass.AutoSize = true;
            this.checkPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkPass.ForeColor = System.Drawing.Color.LimeGreen;
            this.checkPass.Location = new System.Drawing.Point(716, 85);
            this.checkPass.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkPass.Name = "checkPass";
            this.checkPass.Size = new System.Drawing.Size(105, 34);
            this.checkPass.TabIndex = 30;
            this.checkPass.Text = "View";
            this.checkPass.UseVisualStyleBackColor = true;
            this.checkPass.CheckedChanged += new System.EventHandler(this.checkPass_CheckedChanged);
            // 
            // PasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 256);
            this.Controls.Add(this.checkPass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ConfirmPass);
            this.Controls.Add(this.passcode);
            this.Controls.Add(this.emailLabel);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "PasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PasswordForm";
            this.Load += new System.EventHandler(this.PasswordForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox passcode;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Button ConfirmPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkPass;
    }
}