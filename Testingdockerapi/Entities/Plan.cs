﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Testingdockerapi.Entities
{
    public class Plan
    {
        public string clientId { get; set; }
        public Client client { get; set; }
        public List<Cashflow> cashflows { get; set; }
        public List<Account> accounts { get; set; }

        public Allocations allocations {get;set;}
    }
}
