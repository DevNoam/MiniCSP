using _365.Core.Database;
using _365.Core.Properties;
using System.Text.Json;

namespace _365.Core
{
    public class ExportToJSON
    {
        AccountListEntry[] accountsEntries;
        List<AccountProp> accounts = new List<AccountProp>();

        public void Export()
        {
            //DB PASSWORD
            if (!DatabaseManager.Init(true))
            {
                string userPassIn = SimplePasswordField.RequestPassword();
                if (!DatabaseManager.Init(true, userPassIn))
                { 
                    MessageBox.Show("Password incorrect", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var option  = MessageBox.Show("Press Ok to start exporting, will create a file in DB location.", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (option == DialogResult.Cancel)
                return;

            int entriesNum = DatabaseManager.GetAccountsCount();
            accountsEntries = DatabaseManager.FetchAllEntries();

            if (accountsEntries.Length != entriesNum)
            {
                MessageBox.Show("Account list does not makes scense.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (AccountListEntry account in accountsEntries)
            { 
                AccountProp prop = DatabaseManager.FetchAccount(account.id);
                if (prop != null && account.id != -1)
                    accounts.Add(prop);
            }

            var json = JsonSerializer.Serialize(accounts).ToString();

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

            string dateNow = DateTime.Now.ToString("dd.MM.yyyy HH.mm");

            string directoryName = Path.GetDirectoryName(_InitializeDatabase.GetDatabaseLocation().path);
            DirectoryInfo finalDir = Directory.CreateDirectory(Path.Combine(directoryName, "Exports"));

            string filePath = Path.Combine(finalDir.FullName, $"export_{dateNow}.json");
            string finalJson = JsonSerializer.Serialize(jsonElement, options);
            string credits1 = ".--------------------------------------------------------.\r\n|                                                        |\r\n|    __  __ _       _  ____ ____  ____    _              |\r\n|   |  \\/  (_)_ __ (_)/ ___/ ___||  _ \\  | |__  _   _    |\r\n|   | |\\/| | | '_ \\| | |   \\___ \\| |_) | | '_ \\| | | |   |\r\n|   | |  | | | | | | | |___ ___) |  __/  | |_) | |_| |   |\r\n|   |_|__|_|_|_| |_|_|\\____|____/|_|     |_.__/ \\__, |   |\r\n|   |  _ \\  _____   _| \\ | | ___   __ _ _ __ ___|___/    |\r\n|   | | | |/ _ \\ \\ / /  \\| |/ _ \\ / _` | '_ ` _ \\        |\r\n|   | |_| |  __/\\ V /| |\\  | (_) | (_| | | | | | |       |\r\n|   |____/ \\___| \\_/ |_| \\_|\\___/ \\__,_|_| |_| |_|       |\r\n|\t\t\t\t\t\t\t                                           |\r\n|    Database export for MiniCSP Developed by\t\t         |\r\n|    DevNoam (noamsapir.me)\t\t\t\t                       |\r\n|\t\t\t\t\t\t\t                                           |\r\n'--------------------------------------------------------'";
            string credits2 = $"Export date: {DateTime.Now}.";
            string creditsFinal = credits1 + "\r" + credits2;
            finalJson = creditsFinal + "\r" + finalJson;
            try
            {
                File.WriteAllText(filePath, finalJson);
            }
            catch (Exception)
            {
                MessageBox.Show("Generic error, check for read permissions and try again.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                return;
            }
            MessageBox.Show("Done, exported: " + entriesNum + " Entries.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
    }
}
