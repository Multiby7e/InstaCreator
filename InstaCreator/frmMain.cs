using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstaCreator //Coded by Multibyte - Hackforums.net
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            MessageBox.Show("Coded by Multibyte - Hackforums.net");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            Account Acc = new Account(txtUser.Text, txtPass.Text, txtEmail.Text);
        }
    }
}
