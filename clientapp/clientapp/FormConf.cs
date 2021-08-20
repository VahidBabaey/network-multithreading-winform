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
    public partial class FormConf : Form
    {
        public FormConf()
        {
            InitializeComponent();
        }

        private void FormConf_Load(object sender, EventArgs e)
        {
            ShowClients();
            Common.Manager.ClientRecieved += Manager_ClientRecieved;
            Common.Manager.MultiCastMessageRecived += Manager_MultiCastMessageRecived;
        }

        void Manager_MultiCastMessageRecived(object sender, EventArgs e)
        {
            ListMessage.Items.Add(sender.ToString());
        }

        void Manager_ClientRecieved(object sender, EventArgs e)
        {
            ShowClients();
        }

        private void ShowClients()
        {
            ListClients.DataSource = Common.Manager.ListClients.ToList();
        }

        private void Send_Click(object sender, EventArgs e)
        {
            Common.Manager.SendMultyCast(txtMessage.Text.Trim());
            txtMessage.Text = "";
        }
    }
}
