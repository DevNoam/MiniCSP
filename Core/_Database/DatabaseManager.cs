using _365.Core.Database.Functions;
using _365.Core.Properties;

namespace _365.Core.Database
{
    public static class DatabaseManager
    {
        public static string connectionString;

        public static AccountListEntry[] SearchEntries(string search) => _SearchFetch.SearchFetch(search);
        public static int GetAccountsCount() => _GetAccountCount.GetAccountCount();
        public static AccountListEntry[] FetchAllEntries() => _FetchAllAccount.FetchAllAccounts();
        public static AccountProp FetchAccount(int id) => _FetchAccount.Fetch(id);
        public static bool Init(string path) => _InitializeDatabase.Init(path);
    }
}
