using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using MyNet;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;

namespace serverapp
{
    public partial class Form1 : Form
    {
        ClientManager Manager = null;
        bool IsFiil = false;
        public Form1()
        {
            InitializeComponent();
            Manager = new ClientManager();
            Manager.ClientRecived += Manager_ClientRecieved;
            Manager.MessageRecived += Manager_MessageRecieved;
            Manager.ClientRemoved += Manager_ClientRemoved;
            Manager.MessageUDPRecived += Manager_MessageUdpRecieve;
            Manager.FileRecived += Manager_FileRecieved;
            Manager.ScreenRecieved += Manager_ScreenRecieved;
        }

        void Manager_ScreenRecieved(object sender, EventArgs e)
        {
            try
            {
                using (MemoryStream _memory = new MemoryStream(sender as byte[]))
                {
                    pictureBox1.Image = Image.FromStream(_memory);
                    _memory.Flush();
                    _memory.Close();
                    _memory.Dispose();
                    sender = null;
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                    GC.WaitForFullGCComplete();
             
                }
            }
            catch (Exception)
            {

            }
        }

        void Manager_FileRecieved()
        {
            ShowFiles();
        }

        private void ShowFiles()
        {
            if (IsFiil)
                return;
            if (ListClient.SelectedIndex == -1)
                return;
            string ID = ListClient.SelectedValue.ToString().Trim();
            ListFile.DataSource = Manager.GetFiles(ID);
        }

        void Manager_MessageUdpRecieve(string name)
        {
            ListudpMessage.Items.Add(name);
        }

        private void Manager_ClientRemoved(string name)
        {
            ShowClients();
            MessageBox.Show("حذف شد");
        }

        private void Manager_MessageRecieved()
        {
            ShowMessages();
        }

        private void ShowMessages()
        {
            if (IsFiil)
                return;
            if (ListClient.SelectedIndex == -1)
                return;
            string ID = ListClient.SelectedValue.ToString().Trim();
            ListMessage.DataSource = Manager.GetMessages(ID);
        }

        void Manager_ClientRecieved()
        {
            IsFiil = true;
            ShowClients();
            IsFiil = false;
        }

        private void ShowClients()
        {
            ListClient.DataSource = Manager.ListClient.ToList();
            ListClient.DisplayMember = "ClientName";
            ListClient.ValueMember = "ID";
            ListClient.SelectedIndex = -1;
        }

        private void Listen_Click(object sender, EventArgs e)
        {
            Manager.RegisterToDNS(txtDNS.Text.Trim());
            Manager.Start();
        }
        
        private void Send_Click(object sender, EventArgs e)
        {
            if (ListClient.SelectedIndex == -1)
                return;
            Manager.SendMessage(ListClient.SelectedValue.ToString().Trim(), txtMessage.Text.Trim());
        }

        private void SendBroadCast_Click(object sender, EventArgs e)
        {
            Manager.SendBroadCast(txtMessage.Text.Trim());
            txtMessage.Text = "";
        }

        private void ListFile_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory + "/Client" + ListClient.SelectedValue.ToString() + "/" + ListFile.SelectedItem.ToString());
        }

        private void ListClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsFiil)
                return;
            ShowMessages();
            ShowFiles();
            Manager.GetScreen(ListClient.SelectedValue.ToString().Trim());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
