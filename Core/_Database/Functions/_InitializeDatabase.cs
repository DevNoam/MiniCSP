/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////
//// Database initializer      //////
/////////////////////////////////////
using _365.Core;
using _365.Core.Database;
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

public static class _InitializeDatabase
{
    public const string dbName = "365DB.db";
    private const string EncryptionKey = "Bezeq0Nezeq!";
    private static readonly string registryLocation = $@"Software\DevNoam\{Application.ProductName}";
    public static bool Initialize(bool checkConnectionAndPassword = false, string password = "")
    {
        RegistryValues registryProp = GetDatabaseLocation();
        if (registryProp.path == null)
            return false;

        if (checkConnectionAndPassword)
        {
            registryProp.password = password;
        }
        else
        {
            DatabaseManager.connectionString = $"Data Source={registryProp.path};password={registryProp.password}";
        }

        Directory.CreateDirectory(Path.GetDirectoryName(registryProp.path));
        if (!File.Exists(registryProp.path) && !checkConnectionAndPassword)
        { 
            return CreateNewDatabase();
        }

        //Validate if database directory exist, if not, create a new one. This can occur if database deleted.
        return CheckExistingDatabase(registryProp, checkConnectionAndPassword);
    }

    private static bool CreateNewDatabase()
    {
        try
        {
            using (var connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                var createTableSql = @"CREATE TABLE IF NOT EXISTS `Account` (
                `Id` INTEGER NOT NULL UNIQUE,
                `CustomerName` TEXT NOT NULL,
                `Domain` TEXT,
                `Email` TEXT,
                `Password` TEXT,
                `MFA` TEXT,
                `CRM` BLOB NOT NULL,
                `Phone` TEXT,
                `Notes` TEXT,
                `CreationDate` TEXT NOT NULL,
                `ModifyDate` TEXT,
                `Logs` BLOB,
                `isArchived` INTEGER,
                PRIMARY KEY(`Id`)
            )";

                using (var command = new SqliteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            //MessageBox.Show("Database and table created successfully.");
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating database: {ex.Message}");
            return false;
        }
    }

    private static bool CheckExistingDatabase(RegistryValues registryProp, bool checkConnectionAndPassword)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={registryProp.path};password={registryProp.password}"))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM `Account`";
                    command.ExecuteScalar();
                }
                return true;
            }
        }
        catch (Exception ex)
        {
            if (checkConnectionAndPassword)
                return false;

            MessageBox.Show("Generic error or probably a bad password: " + ex.Message);
            var path = SetDatabaseLocation().path;
            if (path != null)
            {
                Application.Restart();
                return false;
            }
            else
                return false;
        }
    }


    public static RegistryValues GetDatabaseLocation(bool setDatabaseOnFail = true)
    {
        var registryProp = new RegistryValues();
        var pathKey = Registry.CurrentUser.CreateSubKey(registryLocation);
        var passKey = Registry.CurrentUser.CreateSubKey(registryLocation);
        var pathObject = pathKey.GetValue("path") ?? string.Empty;
        var passObject = passKey.GetValue("password") ?? string.Empty;

        registryProp.path = pathObject?.ToString() ?? string.Empty;
        registryProp.password = passObject?.ToString() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(registryProp.password))
            registryProp.password = RegistryDecrypt(registryProp.password);

        if (!string.IsNullOrEmpty(registryProp.path) && File.Exists(registryProp.path))
        {
            return registryProp;
        }
        else if (string.IsNullOrEmpty(registryProp.path) && setDatabaseOnFail)
        {
            SetDatabaseLocation();
            return GetDatabaseLocation();
        }
        else
            return registryProp;
    }

    private static RegistryValues SetDatabaseLocation(bool initialSet = true, string initialFolder = null)
    {
        var dialog = new FolderBrowserDialog
        {
            Description = "Select Database location",
            ShowPinnedPlaces = true,
            UseDescriptionForTitle = true,
            InitialDirectory = initialFolder
        };

        var dialogResult = dialog.ShowDialog();
        string path = string.Empty;

        if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
        {
            path = dialog.SelectedPath.Trim();
            string[] subFiles = Directory.GetFiles(path);

            if (File.Exists(Path.Combine(path, dbName)))
            {
                var value = new RegistryValues();
                value.path = Path.Combine(path, dbName);
                value.password = string.Empty;
                SetRegistry("path", value.path);
                bool noPassword = Initialize(true, "");
                if (!noPassword)
                {
                    value.password = SimplePasswordField.NewDatabasePassword();
                    if (!Initialize(true, value.password))
                    {
                        SetDatabaseLocation(true);
                        return null;
                    }
                    value.password = RegistryEncrypt(value.password);
                }

                SetRegistry("password", value.password);
                return value;
            }

            var selection = MessageBox.Show("A new Database will be created in the folder: " + path, Application.ProductName, MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Question);
            if (selection == DialogResult.Retry)
                return SetDatabaseLocation();
            else if (selection == DialogResult.Continue)
            {
                var value = new RegistryValues();
                value.password = SimplePasswordField.NewDatabasePassword();
                value.path = Path.Combine(path, dbName);
                SetRegistry("path", value.path);
                SetRegistry("password", RegistryEncrypt(value.password));
                return value;
            }
            else
            {
                if (initialSet)
                    Application.Exit();
                return new RegistryValues() { path = string.Empty };
            }
        }
        else if (dialogResult == DialogResult.Cancel || string.IsNullOrEmpty(dialog.SelectedPath))
            return new RegistryValues() { path = "error" };

        void SetRegistry(string key, string value)
        {
            var _key = Registry.CurrentUser.CreateSubKey(registryLocation);
            if (value == null)
                value = "";
            _key.SetValue(key, value);
            _key.Close();
        }
        return new RegistryValues() { path = string.Empty };
    }

    public static void ReplaceDatabaseLocation()
    {
        try
        {
            var path = GetDatabaseLocation(false).path;
            var result = SetDatabaseLocation(false, path).path;

            if (result != "error")
            {
                Application.Restart();
                Application.Exit();
            }
        }
        catch (Exception)
        {
            Application.Restart();
            Application.Exit();
            throw new NotImplementedException("Database location replacement exception. Report to Noam (contact@noamsapir.me) with logs below.");
        }
    }


    public static string RegistryEncrypt(string plainText)
    {
        // Convert the plaintext string to a byte array
        byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

        // Convert the encryption key string to a byte array
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(EncryptionKey);

        // Encrypt the plaintext byte array using XOR
        for (int i = 0; i < plainBytes.Length; i++)
        {
            plainBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        // Convert the encrypted byte array back to a string
        return Convert.ToBase64String(plainBytes);
    }

    public static string RegistryDecrypt(string cipherText)
    {
        // Convert the Base64-encoded ciphertext string to a byte array
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        // Convert the encryption key string to a byte array
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(EncryptionKey);

        // Decrypt the ciphertext byte array using XOR
        for (int i = 0; i < cipherBytes.Length; i++)
        {
            cipherBytes[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }

        // Convert the decrypted byte array back to a string
        return System.Text.Encoding.UTF8.GetString(cipherBytes);
    }
}