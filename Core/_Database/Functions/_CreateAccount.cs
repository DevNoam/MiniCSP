using _365.Core.Properties;
using Microsoft.Data.Sqlite;

namespace _365.Core.Database.Functions
{
    public static class _CreateAccount
    {
        internal static int Create(Properties.NewEntry entry)
        {
            string query = $@"
                INSERT INTO Account (CustomerName, CRM, DomainMicrosoft, ResellerTenant, ResellerParent, CreationDate)
                VALUES ('{entry.customerName}', '{entry.crmNumber}', '{entry.tenantDomain}', '{Convert.ToInt32(entry.reseller)}', '{entry.resellerCRMParent}', '{DateTime.Now}');
                SELECT last_insert_rowid();";

            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}
