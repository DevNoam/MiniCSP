using _365.Core.Database;
using _365.Core.Database.Functions;
using _365.Core.Properties;
using System.Text.Json;

namespace _365.Core
{
    public class ImportFromJson
    {
        public void ImportJson(int existedEntries)
        {
            //DB PASSWORD
            //string userPassIn = "password";
            //if (!DatabaseManager.Init(userPassIn))
            //return;


            var option = MessageBox.Show("Importing won't override existing entries but rather create new entries. Place 'import.json' file to the DB folder.", Application.ProductName, MessageBoxButtons.OKCancel);
            if (option == DialogResult.Cancel)
                return;

            string databaseLocation = Path.GetDirectoryName(_InitializeDatabase.GetDatabaseLocation());
            string filePath = Path.Combine(databaseLocation, "import.json");
            if (!Path.Exists(filePath))
            { 
                MessageBox.Show("import.json file not present", Application.ProductName, MessageBoxButtons.OK);
                return;
            }
            string jsonFile = File.ReadAllText(filePath);

            //Begin import
            List<AccountProp> accounts = new List<AccountProp>();

            int startIndex = jsonFile.IndexOfAny(new char[] { '{', '[' });
            if (startIndex != -1)
            {
                string jsonSubstring = jsonFile.Substring(startIndex);
                accounts = JsonSerializer.Deserialize<List<AccountProp>>(jsonSubstring);
            }


            //Backup the DB.
            if (existedEntries > 0)
            {
                DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(databaseLocation, "backup"));
                string dbToCopy = Path.Combine(databaseLocation, _InitializeDatabase.dbName);
                try
                {
                    File.Copy(dbToCopy, Path.Combine(dir.FullName, DateTime.Now.ToString("dd.MM.yyyy HH.mm") + "_" + _InitializeDatabase.dbName));
                }
                catch (Exception)
                {
                    MessageBox.Show("Generic error, check for read permissions and try again.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                    return;
                }
            }


            List<NewEntry> importFailed = new List<NewEntry>();
            int importedSuccess = 0;
            foreach (var account in accounts)
            {
                NewEntry entry = new NewEntry() { 
                    crmNumber = account.crmNumber,
                    customerName = account.customerName
                };
                int creation = DatabaseManager.CreateAccount(entry);
                if (creation == -1)
                { 
                    importFailed.Add(entry);
                    continue;
                }
                

                var changedProps = new Dictionary<string, object>();
                changedProps["Email"] = account.email ?? string.Empty;
                changedProps["Password"] = account.password ?? string.Empty;
                changedProps["MFA"] = account.mfaToken ?? string.Empty;
                changedProps["Phone"] = account.phone ?? string.Empty;
                changedProps["Notes"] = account.notes ?? string.Empty;
                changedProps["isArchived"] = account.isArchived;
                changedProps["ModifyDate"] = account.modifyDate;
                changedProps["CreationDate"] = account.creationDate;
                changedProps["Domain"] = account.domain ?? string.Empty;

                bool isSuccess = DatabaseManager.UpdateAccount(creation, changedProps);
                if (!isSuccess)
                    importFailed.Add(entry);
                else
                    importedSuccess++;
            }

            //Show failed
            var json = JsonSerializer.Serialize(importFailed).ToString();

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            if (importFailed.Count > 0)
            { 
                string filePathOfFailed = Path.Combine(Path.GetDirectoryName(_InitializeDatabase.GetDatabaseLocation()), "failed.json");
                string finalJson = JsonSerializer.Serialize(jsonElement, options);
                File.WriteAllText(filePathOfFailed, finalJson);
            }
            MessageBox.Show("Finished imprting, imported: " + importedSuccess + " / " + accounts.Count() + " Filed: " + importFailed.Count(), Application.ProductName);
        }
    }
}
