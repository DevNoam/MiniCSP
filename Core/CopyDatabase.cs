using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _365.Core
{
    public class CopyDatabase
    {
        public static bool Copy()
        {
            string databaseLocation = Path.GetDirectoryName(_InitializeDatabase.GetDatabaseLocation().path);
            DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(databaseLocation, "Backup"));
            string dbToCopy = Path.Combine(databaseLocation, _InitializeDatabase.dbName);
            try
            {
                File.Copy(dbToCopy, Path.Combine(dir.FullName, DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss") + "_" + _InitializeDatabase.dbName));
            }
            catch (Exception)
            {
                MessageBox.Show("Generic error backing up database, check for read permissions and try again.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                return false;
            }
            return true;
        }
    }
}
