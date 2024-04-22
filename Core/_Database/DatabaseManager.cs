using _365.Core.Database.Functions;
using _365.Core.Properties;

namespace _365.Core.Database
{
    public static class DatabaseManager
    {
        public static string connectionString;

        public static bool Init(string password = "") => _InitializeDatabase.Init(password);
        public static void ReplaceDatabase() => _InitializeDatabase.ReplaceDatabaseLocation();
        public static AccountListEntry[] SearchEntries(string search) => _SearchFetch.SearchFetch(search);
        public static int GetAccountsCount() => _GetAccountCount.GetAccountCount();
        public static AccountListEntry[] FetchAllEntries() => _FetchAllAccount.FetchAllAccounts();
        public static AccountProp FetchAccount(int id) => _FetchAccount.Fetch(id);
        public static bool UpdateAccount(int id, Dictionary<string, object> changedProps, string logToAdd = null) => _UpdateAccount.Update(id, changedProps, logToAdd);
        public static int CreateAccount(NewEntry entry) => _CreateAccount.Create(entry);
        public static bool EditCoreAccount(int id, NewEntry changedProps) => _EditCoreAccount.UpdateAccount(id, changedProps);
    }
}
