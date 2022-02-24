using System;
using System.Collections.Generic;
using System.Text;

namespace Testingdockerapi.Entities
{
    public class Plan
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public Client client { get; set; }
        public Goal goal { get; set; }
        public List<Cashflow> cashflows { get; set; }
        public List<Account> accounts { get; set; }
    }
}
