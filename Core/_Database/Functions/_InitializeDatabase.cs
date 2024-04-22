/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////
//// Database initializer      //////
/////////////////////////////////////


using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace _365.Core.Database.Functions
{
    public static class _InitializeDatabase
    {
        public const string dbName = "365DB.db";
        const string dbsecret = "";
        private readonly static string registryLocation = $@"Software\DevNoam\{Application.ProductName}";
        internal static bool Init(string inKey = "")
        {
            string path = GetDatabaseLocation();
            if (path == null)
                return false;

            string databasePath = path;
            DatabaseManager.connectionString = $"Data Source={databasePath}";


            //Encryption?
            //requires nuget package: SQLitePCLRaw.bundle_e_sqlcipher
            //DatabaseManager.connectionString = new SqliteConnectionStringBuilder(DatabaseManager.connectionString)
            //{
            //    Mode = SqliteOpenMode.ReadWriteCreate
            //    Password = @"J<_x9E95wYXN£~e8Fs=L8gKD0,2[E\\"
            //}.ToString();


            if (!File.Exists(databasePath))
            {
                Console.WriteLine("Database does not exist. Creating a new one...");
                try
                {
                    // Create a new SQLite database file
                    using (var connection = new SqliteConnection(DatabaseManager.connectionString))
                    {
                        connection.Open();

                        // Execute the CREATE TABLE statement to create the "Account" table
                        var createTableSql = @"CREATE TABLE ""Account"" (
	                    ""Id""	INTEGER NOT NULL UNIQUE,
	                    ""CustomerName""	TEXT NOT NULL,
	                    ""Domain""	TEXT,
	                    ""Email""	TEXT,
	                    ""Password""	TEXT,
	                    ""MFA""	TEXT,
	                    ""CRM""	BLOB NOT NULL,
	                    ""Phone""	TEXT,
	                    ""Notes""	TEXT,
	                    ""CreationDate""	TEXT NOT NULL,
	                    ""ModifyDate""	TEXT,
	                    ""Logs""	BLOB,
	                    ""isArchived""	INTEGER,
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

        public static string GetDatabaseLocation()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(registryLocation);
            object pathObject = key.GetValue("path") ?? string.Empty;
            string path = pathObject?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                return path;
            }
            else if (string.IsNullOrEmpty(path))
            { 
                SetDatabaseLocation();
                return GetDatabaseLocation();
            }else
                return path;
        }


        private static string SetDatabaseLocation(bool initialSet = true)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Database location";
            dialog.ShowPinnedPlaces = true;
            dialog.UseDescriptionForTitle = true;

            DialogResult dialogResult = dialog.ShowDialog();
            string path = string.Empty;

            if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                //Set path
                path = dialog.SelectedPath.Trim();
                string[] subFiles = Directory.GetFiles(path);

                if (File.Exists(Path.Combine(path, dbName)))
                        return SetRegistry(path);

                var selection = MessageBox.Show("A new Database will be created in the folder: " + path, Application.ProductName, MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Question);
                if (selection == DialogResult.Retry)
                    return SetDatabaseLocation();
                else if (selection == DialogResult.Continue)
                    return SetRegistry(path);
                else
                {
                    if (initialSet)
                    { 
                        Application.Exit();
                    }
                    return string.Empty;
                }
            }

            string SetRegistry(string path)
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryLocation);
                path = Path.Combine(path, dbName);
                key.SetValue("path", path);
                key.Close();

                return path;
            }
            return string.Empty;
        }
        public static void ReplaceDatabaseLocation()
        {
            SetDatabaseLocation(false);
            Application.Restart();
            Application.Exit();
        }
    }
}