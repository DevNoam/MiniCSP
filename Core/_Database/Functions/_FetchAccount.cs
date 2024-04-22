/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////
//// This function fetches     //////
////  specific account         //////
/////////////////////////////////////
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using System.Data;

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

                            if (!reader.IsDBNull(reader.GetOrdinal("ModifyDate")))
                                DateTime.TryParse(reader.GetString(reader.GetOrdinal("ModifyDate")), out modDate);
                            else
                                DateTime.TryParse(reader.GetString(reader.GetOrdinal("CreationDate")), out modDate);

                            account = new AccountProp
                            {
                                id = id,
                                customerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                                domain = reader.IsDBNull(reader.GetOrdinal("Domain")) ? null : reader.GetString(reader.GetOrdinal("Domain")),
                                email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                                mfaToken = reader.IsDBNull(reader.GetOrdinal("MFA")) ? null : reader.GetString(reader.GetOrdinal("MFA")),
                                crmNumber = reader.IsDBNull(reader.GetOrdinal("CRM")) ? null : reader.GetString(reader.GetOrdinal("CRM")),
                                phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                                notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                modifyDate = reader.IsDBNull(reader.GetOrdinal("ModifyDate")) ? DateTime.Now : DateTime.Parse(reader.GetString(reader.GetOrdinal("ModifyDate"))),
                                isArchived = reader.IsDBNull(reader.GetOrdinal("isArchived")) ? 0 : reader.GetInt32(reader.GetOrdinal("isArchived"))
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
