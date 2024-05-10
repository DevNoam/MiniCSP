/////////////////////////////////////
////DevNoam (noamsapir.me) 2024  ////
using Microsoft.Data.Sqlite;
using System.Web;

namespace _365.Core.Database.Functions
{
    public static class _DeleteAccount
    {
        internal static bool Delete(int id)
        {
            string query = $@"
               DELETE FROM Account WHERE Id = {id}";

            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    try
                    {
                        command.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
    }
}
