using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Account
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public string name { get; set; }
        public double marketValue { get; set; }
        public bool isIncluded { get; set; }
    }
}
