
namespace _365.Core.Properties
{
    public class AccountListEntry
    {
        public int id { get; set; }
        public string customerName { get; set; }
        public override string ToString()
        {
            return customerName;
        }
    }
}
