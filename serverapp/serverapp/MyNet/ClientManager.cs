using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace MyNet
{

    public delegate void MyEvent();
    public delegate void MyEvent2(string name);
    public class ClientManager
    {
        Socket ServerSocket = null;
        SynchronizationContext MainContext = SynchronizationContext.Current;
        //ManualResetEvent Signal = null;
        public List<Client> ListClient { get; private set; }

        public event MyEvent ClientRecived = null;
        public event MyEvent MessageRecived = null;
        public event MyEvent FileRecived = null;
        public event MyEvent2 ClientRemoved = null;
        public event MyEvent2 MessageUDPRecived = null;
        int port = 6767;
        public ClientManager()
        {
            ListClient = new List<Client>();
           // Signal = new ManualResetEvent(false);
        }

        public void Start()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 6060);
            ServerSocket.Bind(ip);

            //Thread t = new Thread(() =>
            //{
                AcceptClients();
            //});
            //t.Name = "Listen";
            //t.IsBackground = true;
            //t.Start();

          //  SendAck();
          // CheckConnection();
            ListenUDP();
        }

        public void RegisterToDNS(string DNSName)
        {
            Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipdns = Dns.Resolve(DNSName).AddressList[0].MapToIPv4();
            _socket.Connect(new IPEndPoint(ipdns, 2020));
            string Message = "Register;" + Dns.GetHostName().Trim();
            _socket.Send(Message.ConverttobyteAscii());
        }
        //private void CheckConnection()
        //{
        //    Thread t1 = new Thread(() =>
        //    {
        //        while (true)
        //        {
        //            for (int i = 0; i < ListClient.Count; i++)
        //            {
        //                if (!ListClient[i].ClientSocket.Isconnected())
        //                {

        //                    string name = ListClient[i].ClientName;
        //                    ListClient.Remove(ListClient[i]);
        //                    if (ClientRemoved != null)
        //                    {
        //                        MainContext.Post((p) => { ClientRemoved(name); }, null);
        //                    }
        //                    Signal.Set();
        //                }
        //            }
        //        }
        //    });
        //    t1.Name = "Connection";
        //    t1.IsBackground = true;
        //    t1.Start();

        //}

        private async void AcceptClients()
        {

                ServerSocket.Listen(1);

                Socket ClientSocket = await Task.Run(() =>
                {

                    return ServerSocket.Accept();

                });

                CheckSecure(ClientSocket);
                AcceptClients();
            //while (true)
            //{
            //    ServerSocket.Listen(1);

            //    Socket ClientSocket = ServerSocket.Accept();


            //    CheckSecure(ClientSocket);

            //}
        }

        private async void CheckSecure(Socket ClientSocket)
        {
            var username = await Task.Run(() =>
            {
                ClientSocket.Send("secure;unknow".ConverttobyteUnicode());
                byte[] buffer = new byte[1024];
                bool IsCheck = true;
                string _username = "";
                while (IsCheck)
                {
                    int Count = ClientSocket.Receive(buffer);
                    if (Count > 0)
                    {
                        string _message = buffer.ConverttoStringUnicode(Count);
                        if(_message.Contains("login"))
                        {
                            _username = _message.Split(';')[1];
                            string _password = _message.Split(';')[2];
                            if (new PrincipalContext(ContextType.Machine, Dns.GetHostName()).ValidateCredentials(_username, _password))
                            {
                                ClientSocket.Send("secure;Accept".ConverttobyteUnicode());
                                IsCheck = false;
                            }
                            else
                            {
                                ClientSocket.Send("secure;notok".ConverttobyteUnicode());
                            }
                        }
                        else
                        {
                            ClientSocket.Send("secure;unknow".ConverttobyteUnicode());

                        }
                    }
                }
                return _username;
            });
            Client _client = AddClient(ClientSocket, username);
            GetMessagesFromClient(_client);

        }

        private async  void GetMessagesFromClient(Client Client)
        {
            await Task.Run(() =>
            {


                byte[] Buffer = new byte[1024];
                int Count = Client.ClientSocket.Receive(Buffer);
                if (Count > 0)
                {
                    string _message = Buffer.ConverttoStringAscii(Count);
                    if (_message.Contains("*S*"))
                    {
                        int StartOfData = _message.IndexOf(";");
                        string FileName = _message.Substring(3, StartOfData - 3);
                        StartOfData++;
                        if (!Directory.Exists(Client.Path))
                        {
                            Directory.CreateDirectory(Client.Path);
                        }
                        BinaryWriter _file = new BinaryWriter(File.Open(Client.Path + "/" + FileName, FileMode.Append));
                        if (_message.Contains("*F*;;;"))
                        {
                            _file.Write(Buffer, StartOfData, Count - StartOfData - 6);
                        }
                        else
                        {
                            _file.Write(Buffer, StartOfData, Count - StartOfData);
                            bool IsRead = true;
                            while (IsRead)
                            {
                                Count = Client.ClientSocket.Receive(Buffer);
                                _message = Buffer.ConverttoStringAscii(Count);
                                if (_message.Contains("*F*;;;"))
                                {
                                    _file.Write(Buffer, 0, Count - 6);
                                    IsRead = false;
                                }
                                else
                                {
                                    _file.Write(Buffer, 0, Count);
                                }
                            }
                            _file.Close();
                            Client.ListFiles.Add(FileName);
                        }
                    }
                    else
                    {
                        _message = Buffer.ConverttoStringUnicode(Count);
                        Client.ListMessage.Add(_message);

                    }
                }
            });
            if (FileRecived != null)
            {
                FileRecived();
            }
            if (MessageRecived != null)
            {
                MessageRecived();
            }

           // });
           // t.Name = Client.ClientName;
           // t.IsBackground = true;
           // t.Start();
           GetMessagesFromClient(Client);
        }


        //private async  void SendAck()
        //{
        //    Task.Run(() =>
        //    {

        //        while (true)
        //        {
        //            Signal.WaitOne();
        //            foreach (var item in ListClient)
        //            {
        //                string str = "Count" + ListClient.Count;
        //                item.ClientSocket.Send(str.ConverttobyteUnicode());
        //            }
        //            Signal.Reset();
        //        }

        //    });

        //}

        private Client AddClient(Socket ClientSocket, string _username)
        {
            Client _client = new Client();

            _client.ClientName = _username;
            _client.ClientSocket = ClientSocket;
            Monitor.Enter(ListClient);
            ListClient.Add(_client);
            if (ClientRecived != null)
            {
                RunMainContext(ClientRecived);
            }
            SendClients();
          //  Monitor.Exit(1);
            return _client;
        }

        private void SendClients()
        {
            string str = "Clients;";
            foreach (var item in ListClient)
            {
                str += item.ClientName;
            }
            foreach (var item in ListClient)
            {
                item.ClientSocket.Send(str.ConverttobyteUnicode());
            }
        }

        private void RunMainContext(MyEvent function)
        {
            MainContext.Post((p) => { function(); }, null);
        }
        private void RunMainContext(MyEvent2 function, string Data)
        {
            MainContext.Post((p) => { function(Data); }, null);
        }
        public List<string> GetMessages(string ID)
        {
            return ListClient.SingleOrDefault(p => p.ID == ID).ListMessage.ToList();
        }
        public List<string> GetFiles(string ID)
        {
            return ListClient.SingleOrDefault(p => p.ID == ID).ListFiles.ToList();
        }
        public void SendMessage(string ID, string Message)
        {
            ListClient.SingleOrDefault(p => p.ID == ID).ClientSocket.Send(Message.ConverttobyteUnicode());
        }
        public void ListenUDP()
        {
            Socket _UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 5058);
            EndPoint ipclient = new IPEndPoint(IPAddress.Any, 5058);
            _UdpSocket.Bind(ip);
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int count = _UdpSocket.ReceiveFrom(buffer, ref ipclient);
                    if (count > 0)
                    {
                        string str = buffer.ConverttoStringUnicode(count);
                        str += " از " + ipclient.ToString();
                        if (MessageUDPRecived != null)
                        {
                            RunMainContext(MessageUDPRecived, str);
                        }
                    }
                }
            });

            t.Name = "UdpThread";
            t.IsBackground = true;
            t.Start();
        }

        public void SendBroadCast(string Message)
        {
            Socket BroadCastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 9090);
            BroadCastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            BroadCastSocket.SendTo(Message.ConverttobyteUnicode(), ip);
        }

        Socket ScreenSocket = null;
        public event EventHandler ScreenRecieved = null;
        Thread ScreenThread = null;
        public void GetScreen(string ID)
        {
            if (ScreenThread != null)
            {
                try
                {
                    ScreenThread.Abort();
                }
                catch (Exception)
                {
                }
            }

            var _client = ListClient.SingleOrDefault(p => p.ID == ID);
            ScreenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ScreenSocket.Connect(_client.IP, 7078);
            ScreenThread = new Thread(() =>
            {
                byte[] buffer = new byte[2000 * 90000];
                while (true)
                {
                    int Count = ScreenSocket.Receive(buffer);
                    if(Count > 0)
                    {
                        if(ScreenRecieved != null)
                        {
                            MainContext.Post((p) => { ScreenRecieved(buffer, null); }, null);
                        }
                        Task.Delay(200);
                    }
                }



            });
            ScreenThread.Name = "ScreenThread";
            ScreenThread.IsBackground = true;
            ScreenThread.Start();
        }
    }
}
