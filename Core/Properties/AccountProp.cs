using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _365.Core.Properties
{
    public class AccountProp
    {
        public int id { get; set; }
        public string? customerName { get; set; }
        public string? domain { get; set; }
        public string? domainMicrosoft { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? mfaToken { get; set; }
        public string? crmNumber { get; set; }
        public string? phone { get; set; }
        public string? notes { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime modifyDate { get; set; }
        public int isArchived { get; set; }
        public log[]? logs { get; set; }
    }

    public class log
    { 
        public string? maintainer { get; set; }
        public string? operation { get; set; }
        public DateTime dateOfOperation { get; set; }
    }
}
