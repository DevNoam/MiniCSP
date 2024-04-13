using _365.Core.Properties;
using Microsoft.Data.Sqlite;

namespace _365.Core.Database.Functions
{
    public static class _SearchFetch
    {

        internal static AccountListEntry[] SearchFetch(string search)
        {
            List<AccountListEntry> Accounts = new List<AccountListEntry>();
            // SQL query to retrieve customer entries
            string query = $"SELECT * FROM Account WHERE `CustomerName` LIKE '%{search}%' OR `Domain` LIKE '%{search}%' OR `DomainMicrosoft` LIKE '%{search}%' OR `Email` LIKE '%{search}%' OR `CRM` LIKE '%{search}%' OR `Phone` LIKE '%{search}%'";

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
                            Accounts.Add(new AccountListEntry { id = id, customerName = customerName });
                        }
                    }
                }
            }
            return Accounts.ToArray();
        }
    }
}