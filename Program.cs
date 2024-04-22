/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////

using _365.Core.Database;

namespace _365
{
    internal static class Program
    {
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