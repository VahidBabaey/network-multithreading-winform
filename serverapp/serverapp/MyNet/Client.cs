using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyNet
{
   public class Client
    {
       public Client()
       {
           ListMessage = new List<string>();
           ListFiles = new List<string>();
           ID = Guid.NewGuid().ToString("N");
       }
       public string ClientName { get; set; }

       public string ID { get; set; }

       public string Password { get; set; }

       public Socket ClientSocket { get; set; }

       public List<string> ListMessage { get; set; }

       public List<string> ListFiles { get; set; }

       public  string Path
       {
           get
           {
               return Environment.CurrentDirectory + "/Client" + ID;
           }
       }
       public IPAddress IP
       {
           get
           {
               return IPAddress.Parse(ClientSocket.RemoteEndPoint.ToString().Split(':')[0]);
           }
       }
    }
}
