using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System
{
   public static class ExtentionFuction
    {
       public static byte[] ConverttobyteUnicode(this string str)
       {
           return UnicodeEncoding.Unicode.GetBytes(str);
       }
       public static byte[] ConverttobyteAscii(this string str)
       {
           return ASCIIEncoding.ASCII.GetBytes(str);
       }
       public static string ConverttoStringUnicode(this byte[] buffer, int Count)
       {
           return UnicodeEncoding.Unicode.GetString(buffer, 0, Count);
       }
       public static string ConverttoStringAscii(this byte[] buffer, int Count)
       {
           return ASCIIEncoding.ASCII.GetString(buffer, 0, Count);
       }
       public static bool Isconnected(this Socket Socket)
       {
           return (!Socket.Poll(100, SelectMode.SelectRead) && Socket.Available == 0);
       }


       }
}
