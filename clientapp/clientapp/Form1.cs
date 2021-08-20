using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace clientapp
{
    public partial class Form1 : Form
    {
        MyNet.ServerManager Manager = Common.Manager;
        FormSecure frmlgn = new FormSecure();
        public Form1()
        {
            InitializeComponent();
        }

        void Manager_BroadCastRecieve(object sender, EventArgs e)
        {
            ListBroadCaseMessage.Items.Add(sender.ToString());
        }

        void Manager_MessageRecieve(object sender, EventArgs e)
        {
            ListMessage.Items.Add(sender.ToString());
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            Manager.Connect(ComboListServer.Text.Trim());
            Manager.MessageRecieve += Manager_MessageRecieve;
            Manager.BroadCastRecieve += Manager_BroadCastRecieve;
            Manager.ReqiereSecure += Manager_ReqiereSecure;
            Manager.SuccessSecure += Manager_SuccessSecure;
            Manager.FailSecure += Manager_FailSecure;
        }



        void Manager_FailSecure(object sender, EventArgs e)
        {
            MessageBox.Show("نام کاربری یا رمز عبور اشتباه است","خطا", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            frmlgn = new FormSecure();
            frmlgn.ShowDialog();
        }

        void Manager_SuccessSecure(object sender, EventArgs e)
        {
            frmlgn.Close();
        }

        void Manager_ReqiereSecure(object sender, EventArgs e)
        {
            frmlgn.ShowDialog();
        }
        private void CliseSocket()
        {
            Manager.CloseSocket();
        }

        private void Send_Click(object sender, EventArgs e)
        {
            Manager.SendMessage(txtMessage.Text.Trim());
        }

        private void DisConnet_Click(object sender, EventArgs e)
        {
            Manager.CloseSocket();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager.CloseSocket();
        }

        private void Sendudp_Click(object sender, EventArgs e)
        {
            Manager.SendUDP(txtMessage.Text.Trim());
        }

        private void Recieveudp_Click(object sender, EventArgs e)
        {
            Manager.RecievUdp(txtDNS.Text.Trim());
        }

        private void Update_Click(object sender, EventArgs e)
        {
            Manager.GetServer(txtDNS.Text.Trim());
            ComboListServer.DataSource = Manager.ListServer.ToList();
        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.ShowDialog();
            txtFileName.Text = op.FileName;
        }

        private void SendFile_Click(object sender, EventArgs e)
        {
            Manager.SendFiles(txtFileName.Text.Trim());
        }

        private void Conf_Click(object sender, EventArgs e)
        {
            new FormConf().ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
