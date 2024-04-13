using Microsoft.Data.Sqlite;

namespace _365.Core.Database.Functions
{
    public static class _GetAccountCount
    {

        internal static int GetAccountCount()
        {
            string query = $"SELECT COUNT(DISTINCT id) FROM `Account`";

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
                            int count = reader.GetInt32(0);
                            return count;
                        }
                    }
                }
            }
            return -1;
        }
    }
}