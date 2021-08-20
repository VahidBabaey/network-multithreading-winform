using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace MyNet
{
   public  class ServerManager
    {
       public List<string> ListServer  { get; set; }
       public List<string> ListClients  { get; set; }
       public string UserName { get; set; }
       public event EventHandler MessageRecieve = null;
       public event EventHandler BroadCastRecieve = null;
       public event EventHandler ReqiereSecure = null;
       public event EventHandler SuccessSecure = null;
       public event EventHandler FailSecure = null;
       public event EventHandler ClientRecieved = null;
       public event EventHandler MultiCastMessageRecived = null;
       SynchronizationContext Context = null;
       Socket ServerSocket = null;
       Socket ConfSocket = null;
       Socket ScreenSocket = null;
       IPEndPoint IPMultiCast = null;
       public ServerManager()
       {
           ListServer = new List<string>();
           ListClients = new List<string>();
           Context = SynchronizationContext.Current;
       }
       private void ConfigMulitiCastMessage()
       {
           IPMultiCast = new IPEndPoint(IPAddress.Parse("224.100.10.1"), 8086);
           ConfSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           SendScreen();
           var iplocal = new IPEndPoint(IPAddress.Any, 8086);
           ConfSocket.Bind(iplocal);

           ConfSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(IPAddress.Parse("224.100.10.1")));
           Thread t = new Thread(() =>
           {
               byte[] buffer = new byte[1024];
               while (true)
               {
                   EndPoint ep = (EndPoint)IPMultiCast;
                   ConfSocket.ReceiveFrom(buffer, ref ep);
                   String _message = UnicodeEncoding.Unicode.GetString(buffer);
                   if(MultiCastMessageRecived != null)
                   {
                       Context.Post((p) => { MultiCastMessageRecived(_message, null); }, null);
                   }
              }

           });
           t.IsBackground = true;
           t.Start();
       }
       private void SendScreen()
       {
           ScreenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           ScreenSocket.Bind(new IPEndPoint(IPAddress.Any, 7078));

           Thread t = new Thread(() =>
           {

               while (true)
               {
                   ScreenSocket.Listen(1);
                   var _socket = ScreenSocket.Accept();
                   ThreadPool.QueueUserWorkItem((_) =>
                   {
                       MemoryStream _memory = null;
                       while (true)
                       {
                           try
                           {
                               Bitmap bip = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height);
                               Graphics g = Graphics.FromImage(bip);
                               g.CopyFromScreen(0, 0, 0, 0, new Size(bip.Width, bip.Height));
                               _memory = new MemoryStream();
                               bip.Save(_memory, ImageFormat.Jpeg);
                               byte[] buffer = _memory.GetBuffer();
                               _socket.Send(buffer);
                               _memory.Flush();
                               _memory.Close();
                               buffer = null;
                               Thread.Sleep(200);
                           }
                           catch (Exception)
                           {

                           }
                       }


                   }, null);
               }

           });


           t.Name = "ScreenThread";
           t.IsBackground = true;
           t.Start();

       }
       public void SendMultyCast(string _message)
       {
           ConfSocket.SendTo((UserName + " Says: " + _message).ConverttobyteUnicode(), IPMultiCast);
       }
       public void Connect(string ServerName)
       {
           var ipad = Dns.Resolve(ServerName).AddressList[0].ToString();
           Context = SynchronizationContext.Current;
           ConfigMulitiCastMessage();
           ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipad), 6060);
           ServerSocket.Connect(ip);
           GetMessagesFromServer();
       }
       public void SendMessage(string _message)
       {
           if (ServerSocket != null && ServerSocket.Connected)
               ServerSocket.Send(_message.ConverttobyteUnicode());
       }

       private void GetMessagesFromServer()
       {
           ThreadPool.QueueUserWorkItem((_) =>
           {
               while (true)
               {
                   byte[] Buffer = new byte[1024];

                   int Count = ServerSocket.Receive(Buffer);
                   if (Count > 0)
                   {
                       string _message = Buffer.ConverttoStringUnicode(Count);
                       if (_message.Contains("Clients"))
                       {
                           var list = _message.Split(';');
                           ListClients.Clear();
                           for (int i = 1; i < list.Length; i++)
                           {
                               ListClients.Add(list[i]);
                           }
                           if (ClientRecieved != null)
                           {
                               Context.Post((p) => { ClientRecieved(null, null); }, null);
                           }
                       }
                       if (_message.Contains("secure"))
                       {
                           VerfySecure(_message);
                       }
                       else
                       {
                           if (MessageRecieve != null)
                           {
                               Context.Post((p) => { MessageRecieve(_message, null); }, null);
                           }
                       }
                   }
               }
           });
       }

       private void VerfySecure(string _Message)
       {
           if (_Message.Contains("unknow"))
           {
               if(ReqiereSecure != null)
               {
                   Context.Post((p) => { ReqiereSecure(null, null); }, null);
               }
           }
           if(_Message.Contains("Accept"))
           {
               if(SuccessSecure != null)
               {
                   Context.Post((p) => { SuccessSecure(null, null); }, null);
               }
           }
           if(_Message.Contains("notok"))
           {
               if(FailSecure != null)
               {
                   Context.Post((p) => { FailSecure(null, null); }, null);
               }
           }
       }
       public void CloseSocket()
       {
           if (ServerSocket != null && ServerSocket.Connected)
           {
               ServerSocket.Disconnect(false);
               ServerSocket.Shutdown(SocketShutdown.Both);
               ServerSocket.Close();
           }

       }

       public void SendUDP(string DNSName)
       {
           //Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           //IPEndPoint ip = new IPEndPoint(IPAddress.Parse(), 7070);
           //_socket.SendTo(DNSName.ConverttobyteUnicode(), ip);
       }
       public void RecievUdp(string DnsName)
       {
           Socket _UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           EndPoint ip = new IPEndPoint(IPAddress.Any, 9090);
           EndPoint ipclient = new IPEndPoint(IPAddress.Any, 7070);
           _UdpSocket.Bind(ip);
           Thread t = new Thread(() =>
           {
               while (true)
               {
                   byte[] buffer = new byte[1024];
                   int count = _UdpSocket.ReceiveFrom(buffer, ref ip);
                   if (count > 0)
                   {
                       string str = buffer.ConverttoStringUnicode(count);
                       str += " از " + DnsName.ToString();
                       if (BroadCastRecieve != null)
                       {
                           Context.Post((p) => { BroadCastRecieve(str, null); }, null);
                       }
                   }
               }
           });

           t.Name = "UdpThread";
           t.IsBackground = true;
           t.Start();
       }
       public void GetServer(string DnsName)
       {
           Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           IPAddress ipdns = Dns.Resolve(DnsName).AddressList[0].MapToIPv4();
           _socket.Connect(new IPEndPoint(ipdns, 2020));
           string Message = "Recieve:1" + Dns.GetHostName().Trim();
           _socket.Send(Message.ConverttobyteAscii());
           byte[] buffer = new byte[1024];
           int Count = _socket.Receive(buffer);
           string str = ASCIIEncoding.ASCII.GetString(buffer, 0, Count);
           foreach (var item in str.Split(';'))
           {
               ListServer.Add(item);
           }
       }
       public void SendFiles(string FileName)
       {
           FileInfo f = new FileInfo(FileName);
           byte[] Perfix = ASCIIEncoding.ASCII.GetBytes("*S*");
           byte[] FileNames = ASCIIEncoding.ASCII.GetBytes(f.Name + ";");
           byte[] Data = File.ReadAllBytes(FileName);
           byte[] PostFix = ASCIIEncoding.ASCII.GetBytes("*F*;;;");

           byte[] buffer = new byte[Perfix.Length + FileNames.Length + Data.Length + PostFix.Length];
           Perfix.CopyTo(buffer, 0);
           FileNames.CopyTo(buffer, Perfix.Length);
           Data.CopyTo(buffer, Perfix.Length + FileNames.Length);
           PostFix.CopyTo(buffer, Perfix.Length + FileNames.Length + Data.Length);
           ServerSocket.Send(buffer);
       }
    }
}
