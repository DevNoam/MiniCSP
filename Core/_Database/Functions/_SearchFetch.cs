/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////
//// This function retrives     /////
//// accounts based on search  //////
/////////////////////////////////////
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using System.Web;

namespace _365.Core.Database.Functions
{
    public static class _SearchFetch
    {

        internal static AccountListEntry[] SearchFetch(string search)
        {
            search = HttpUtility.HtmlEncode(search);
            List<AccountListEntry> Accounts = new List<AccountListEntry>();
            // SQL query to retrieve customer entries
            string query = $"SELECT * FROM Account WHERE `CustomerName` LIKE '%{search}%' OR `Domain` LIKE '%{search}%' OR `Email` LIKE '%{search}%' OR `CRM` LIKE '%{search}%' OR `Phone` LIKE '%{search}%'";

            // Open connection to the database
            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                // Execute the SQL query
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        // Iterate through the result set and add customer entries to the list
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var customerName = reader.GetString(1);
                            var isArchivedVar = reader.IsDBNull(reader.GetOrdinal("isArchived")) ? 0 : reader.GetInt32(reader.GetOrdinal("isArchived"));
                            bool isArchived = isArchivedVar == 1 ? true : false;

                            Accounts.Add(new AccountListEntry { id = id, customerName = customerName, isArchived = isArchived });
                        }
                    }
                }
            }
            return Accounts.ToArray();
        }
    }
}