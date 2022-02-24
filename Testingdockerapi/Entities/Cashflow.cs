using System;
using System.Collections.Generic;
using System.Text;

namespace Testingdockerapi.Entities
{
    public enum CashflowType { Income, Expense}
    public class Cashflow
    {
        public int id { get; set; }
        public int ClientId { get; set; }
        public CashflowType type { get; set; }
        public double amount { get; set; }
    }
}
