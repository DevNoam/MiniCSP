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
                //Success
            }
            else
            {
                MessageBox.Show("No connection to the Database", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
                return;
            }
            Application.Run(new AppDash());
        }
    }
}