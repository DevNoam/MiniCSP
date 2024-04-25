using System.Web;

namespace _365.Core.Properties
{
    public class AccountListEntry
    {
        public int id = -1;
        public string customerName { get; set; }
        public bool isArchived { get; set; }
        public override string ToString()
        {
            return HttpUtility.HtmlDecode(customerName);
        }
    }
}
