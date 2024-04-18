using _365.Core.Database;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace _365
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 




        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            
            //DB Init
            if (DatabaseManager.Init())
            {
            }
            else
            {
                MessageBox.Show("No connection to the Database", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //try again?
                
                System.Environment.Exit(0);
            }
            Application.Run(new AppDash());
        }
    }
}