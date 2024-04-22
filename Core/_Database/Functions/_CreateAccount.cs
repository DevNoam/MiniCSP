/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////
//// This function creates     //////
////  core account            ///////
/////////////////////////////////////
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using System.Web;

namespace _365.Core.Database.Functions
{
    public static class _CreateAccount
    {
        internal static int Create(NewEntry entry)
        {
            string query = $@"
                INSERT INTO Account (CustomerName, CRM, Domain, creationDate)
                VALUES ('{HttpUtility.HtmlEncode(entry.customerName)}', '{HttpUtility.HtmlEncode(entry.crmNumber)}', '{HttpUtility.HtmlEncode(entry.tenantDomain)}', '{DateTime.Now.ToString()}');
                SELECT last_insert_rowid();";

            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}
