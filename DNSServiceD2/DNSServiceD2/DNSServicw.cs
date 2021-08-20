using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DNSServiceD2
{
    public partial class DNSServicw : ServiceBase
    {
        Socket _socket = null;
        List<string> ListServer = null;

        public DNSServicw()
        {
            InitializeComponent();
            ListServer = new List<string>();
        }

        protected override void OnStart(string[] args)
        {
            Listen();
        }

        private void Listen()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 2021);
            _socket.Bind(ip);

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    _socket.Listen(1);
                    Socket ClientSocket = _socket.Accept();
                    byte[] buffer = new byte[1024];
                    int Count = ClientSocket.Receive(buffer);
                    if (Count == 0)
                        continue;
                    var _message = ASCIIEncoding.ASCII.GetString(buffer, 0, Count);
                    if (_message.Contains("Register"))
                    {
                        if (ListServer.Contains(_message.Split(':')[1]))
                            ListServer.Add(_message);
                    }
                    else
                    {
                        string str = "";
                        foreach (var item in ListServer)
                        {
                            str += item + ":";
                        }
                        ClientSocket.Send(ASCIIEncoding.ASCII.GetBytes(str));
                    }
                }

            });
            t.IsBackground = true;
            t.Start();
        }

        protected override void OnStop()
        {
        }
    }
}
