/////////////////////////////////////
////DevNoam (noamsapir.me) 2024   ///
/////////////////////////////////////
//// This function updates      /////
//// account fields, not CORE  //////
/////////////////////////////////////
using Microsoft.Data.Sqlite;

namespace _365.Core.Database.Functions
{
    public static class _UpdateAccount
    {
        public static bool Update(int accountId, Dictionary<string, object> changedProps, string logToAdd)
        {
            string query = "UPDATE Account SET ";
            bool first = true;

            // Add changed properties to the query
            foreach (var prop in changedProps)
            {
                if (!first)
                {
                    query += ", ";
                }
                query += $"{prop.Key} = @{prop.Key}";
                first = false;
            }

            // Add WHERE clause
            query += " WHERE Id = @Id";

            // Open connection to the database
            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                // Create and execute the SQL command
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    // Set parameters
                    foreach (var prop in changedProps)
                    {
                        command.Parameters.AddWithValue($"@{prop.Key}", prop.Value);
                    }
                    command.Parameters.AddWithValue("@Id", accountId);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if any rows were affected
                    return rowsAffected > 0;
                }
            }
            //FETCH LOG INFO
            //UPDATE LOG INFO
        }
    }
}
