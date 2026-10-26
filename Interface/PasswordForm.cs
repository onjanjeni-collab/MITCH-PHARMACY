using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Revmax_Interface_Promun
{
    public partial class PasswordForm : Form
    {
        public string Password { get; private set; }
        public PasswordForm()
        {
            InitializeComponent();
        }

        private void email_TextChanged(object sender, EventArgs e)
        {

        }

        private void ConfirmPass_Click(object sender, EventArgs e)
        {
            Password = passcode.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkPass.Checked)
            {
                passcode.PasswordChar = '\0';
                checkPass.Text = "Hide";
            }
            else
            {
                passcode.PasswordChar = '*';
                checkPass.Text = "View";
            }
        }

        private void PasswordForm_Load(object sender, EventArgs e)
        {

        }
    }
}
