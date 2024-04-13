using Microsoft.Data.Sqlite;

namespace _365.Core.Database.Functions
{
    public static class _InitializeDatabase
    {
        internal static bool Init(string path)
        {
            string databasePath = Path.Combine(path, "365DB.db");
            DatabaseManager.connectionString = $"Data Source={databasePath}";

            if (!File.Exists(databasePath))
            {
                Console.WriteLine("Database does not exist. Creating a new one...");
                try
                {
                    // Create a new SQLite database file
                    using (var connection = new SqliteConnection($"Data Source={databasePath}"))
                    {
                        connection.Open();

                        // Execute the CREATE TABLE statement to create the "Account" table
                        var createTableSql = @"
                        CREATE TABLE ""Account"" (
	                    ""Id""	INTEGER NOT NULL UNIQUE,
	                    ""CustomerName""	TEXT,
	                    ""Domain""	TEXT UNIQUE,
	                    ""DomainMicrosoft""	TEXT NOT NULL UNIQUE,
	                    ""Email""	TEXT NOT NULL,
	                    ""Password""	TEXT NOT NULL,
	                    ""MFA""	TEXT,
	                    ""CRM""	BLOB,
	                    ""Phone""	TEXT,
	                    ""Notes""	TEXT,
	                    ""CreationDate""	TEXT NOT NULL,
	                    ""ModifyDate""	TEXT NOT NULL,
	                    ""Logs""	BLOB,
	                    PRIMARY KEY(""Id"")
                        )";

                        using (var command = new SqliteCommand(createTableSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("Database and table created successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating database: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Database exists. No need to create a new one.");
                return true;
            }
        }
    }
}