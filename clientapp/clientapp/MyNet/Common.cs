using MyNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
   public  class Common
    {
       public static ServerManager _Manager = new ServerManager();

       public static ServerManager Manager
      {
       get
          {
              return _Manager;
          }
      }
    }
}
