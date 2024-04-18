using Microsoft.Data.Sqlite;
using _365.Core.Properties;

namespace _365.Core.Database.Functions
{
    public static class _FetchAllAccount
    {
        internal static AccountListEntry[] FetchAllAccounts()
        {
            List<AccountListEntry> Accounts = new List<AccountListEntry>();
            // SQL query to retrieve customer entries
            string query = "SELECT Id, CustomerName FROM Account";

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

                            Accounts.Add(new AccountListEntry { id = id, customerName = customerName});

                        }
                    }
                }
            }
            return Accounts.ToArray();
        }
    }
}
