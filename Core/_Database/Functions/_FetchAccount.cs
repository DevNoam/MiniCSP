using _365.Core.Properties;
using Microsoft.Data.Sqlite;

namespace _365.Core.Database.Functions
{
    public static class _FetchAccount
    {
        internal static AccountProp Fetch(int id)
        {
            //Fetch data
            string query = $"SELECT * FROM Account WHERE Id = {id}";
            AccountProp account = new AccountProp();
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
                            var customerName = reader.GetString(1);
                            DateTime modDate = DateTime.Now;
                            DateTime.TryParse(reader.GetString(reader.GetOrdinal("ModifyDate")), out modDate);

                            account = new AccountProp
                            {
                                id = id,
                                customerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                                domain = reader.GetString(reader.GetOrdinal("Domain")),
                                domainMicrosoft = reader.GetString(reader.GetOrdinal("domainMicrosoft")),
                                email = reader.GetString(reader.GetOrdinal("Email")),
                                password = reader.GetString(reader.GetOrdinal("Password")),
                                mfaToken = reader.GetString(reader.GetOrdinal("MFA")),
                                crmNumber = reader.GetString(reader.GetOrdinal("CRM")),
                                phone = reader.GetString(reader.GetOrdinal("Phone")),
                                notes = reader.GetString(reader.GetOrdinal("Notes")),
                                modifyDate = modDate,
                                isArchived = reader.GetInt32(reader.GetOrdinal("isArchived"))
                                //creationDate = reader.GetString(reader.GetOrdinal("CustomerName")),
                            };
                        }
                    }
                }
            }
            return account;
        }
    }
}
