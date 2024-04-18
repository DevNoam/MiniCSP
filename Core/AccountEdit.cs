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
                changedProps["Email"] = newAccountProp.email;
            }
            if (newAccountProp.password != oldAccountProp.password)
            {
                changedProps["Password"] = newAccountProp.password;
            }
            if (newAccountProp.mfaToken != oldAccountProp.mfaToken)
            {
                changedProps["MFA"] = newAccountProp.mfaToken;
            }
            if (newAccountProp.domainMicrosoft != oldAccountProp.domainMicrosoft)
            {
                changedProps["DomainMicrosoft"] = newAccountProp.domainMicrosoft;
            }
            if (newAccountProp.phone != oldAccountProp.phone)
            {
                changedProps["Phone"] = newAccountProp.phone;
            }
            if (newAccountProp.notes != oldAccountProp.notes)
            {
                changedProps["Notes"] = newAccountProp.notes;
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

            //MessageBox.Show(logInfo);
            if (changedProps.Count <= 0)
                return false;
            return DatabaseManager.UpdateAccount(newAccountProp.id, changedProps, logInfo);
        }
    }
}

