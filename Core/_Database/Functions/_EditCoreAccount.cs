/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
/////////////////////////////////////
//// This function handles     //////
//// core account edit of the ///////
//// CRM and CustomerNumber  ////////
/////////////////////////////////////


using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using System.Web;

namespace _365.Core.Database.Functions
{
    public static class _EditCoreAccount
    {
        public static bool UpdateAccount(int accountId, NewEntry changedProps, string logToAdd = null)
        {
            string query = $@"
                UPDATE Account 
                SET CustomerName = '{HttpUtility.HtmlEncode(changedProps.customerName)}', CRM = '{HttpUtility.HtmlEncode(changedProps.crmNumber)}', ModifyDate = '{DateTime.Now}'
                WHERE Id = '{accountId}'";

            // Open connection to the database
            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                // Create and execute the SQL command
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if any rows were affected
                    return rowsAffected > 0;
                }
            }
        }
    }
}
