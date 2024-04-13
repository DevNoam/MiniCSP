using _365.Core.Database;
using _365.Core.Properties;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _365.Core
{
    public class AccountEdit
    {
        public AccountProp oldAccountProp { get; set; }
        public AccountProp newAccountProp { get; set; }
        public bool PublishEdit(AccountProp newAccountProp)
        {

            var changedProps = new Dictionary<string, object>();
            #region props compare
            if (newAccountProp.id != oldAccountProp.id)
            {
                return false;
            }
            if (newAccountProp.email != oldAccountProp.email)
            {
                changedProps["email"] = newAccountProp.email;
            }
            if (newAccountProp.password != oldAccountProp.password)
            {
                changedProps["password"] = newAccountProp.password;
            }
            if (newAccountProp.mfaToken != oldAccountProp.mfaToken)
            {
                changedProps["mfaToken"] = newAccountProp.mfaToken;
            }
            if (newAccountProp.domainMicrosoft != oldAccountProp.domainMicrosoft)
            {
                changedProps["domainMicrosoft"] = newAccountProp.domainMicrosoft;
            }
            if (newAccountProp.phone != oldAccountProp.phone)
            {
                changedProps["phone"] = newAccountProp.phone;
            }
            if (newAccountProp.notes != oldAccountProp.notes)
            {
                changedProps["notes"] = newAccountProp.notes;
            }
            if (newAccountProp.isArchived != oldAccountProp.isArchived)
            {
                changedProps["isArchived"] = newAccountProp.isArchived;
            }
            #endregion
            changedProps["ModifyDate"] = newAccountProp.modifyDate;

            string changesMade = null;
            foreach (var prop in changedProps)
            {
                changesMade += "`" + prop.Key + "`" + ",";
                //Console.WriteLine($"Upload {prop.Key}: {kpropvp.Value}");
                // Implement logic to update database
            }
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            string logInfo = userName + " made a change to: " + changesMade + " At: " + newAccountProp.modifyDate;

            MessageBox.Show(logInfo);
            if (changedProps.Count <= 0)
                return false;
            return Upload(newAccountProp.id, changedProps, logInfo);
        }


        bool Upload(int accountId, Dictionary<string, object> changedProps, string logToAdd)
        {
            string query = "UPDATE Account SET ";
            bool first = true;

            // Add changed properties to the query
            foreach (var prop in changedProps)
            {
                if (!first)
                {
                    query += ", ";
                }
                query += $"{prop.Key} = @{prop.Key}";
                first = false;
            }

            // Add WHERE clause
            query += " WHERE Id = @Id";

            // Open connection to the database
            using (SqliteConnection connection = new SqliteConnection(DatabaseManager.connectionString))
            {
                connection.Open();

                // Create and execute the SQL command
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    // Set parameters
                    foreach (var prop in changedProps)
                    {
                        command.Parameters.AddWithValue($"@{prop.Key}", prop.Value);
                    }
                    command.Parameters.AddWithValue("@Id", accountId);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if any rows were affected
                    return rowsAffected > 0;
                }
            }
        }
    }
}

