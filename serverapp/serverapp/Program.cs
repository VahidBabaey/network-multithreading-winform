using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace serverapp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] arge)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          //  Application.ThreadException += Application_ThreadException;
          //  AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new Form1());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorHandler.GetError(e.ExceptionObject);
            if (e.IsTerminating)
                Process.Start(Application.ExecutablePath);
        }


        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

            ErrorHandler.GetError(e.Exception);
        }
    }
}
