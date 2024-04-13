using _365.Core.Database;

namespace _365
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    MessageBox.Show("Database path is misconfigured!", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    System.Environment.Exit(0);
            //}

            //DB Init
            if (DatabaseManager.Init(@"C:\Users\noam1\Desktop") == true)
            {
            }
            else
            {
                MessageBox.Show("No connection to the Database", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }


            //If success ->
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}