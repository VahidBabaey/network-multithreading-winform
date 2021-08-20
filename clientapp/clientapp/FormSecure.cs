using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clientapp
{
    public partial class FormSecure : Form
    {
        public FormSecure()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            Common.Manager.UserName = txtUserName.Text.Trim();
            Common.Manager.SendMessage("login;" + txtUserName.Text.Trim() + ";" + txtPassword.Text.Trim());
            Close();
        }
    }
}
