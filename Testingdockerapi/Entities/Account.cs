using System;
using System.Collections.Generic;
using System.Text;

namespace Testingdockerapi.Entities
{
    public class Account
    {
        public string id { get; set; }
        public string clientId { get; set; }
        public string name { get; set; }
        public double marketValue { get; set; }
        public bool isIncluded { get; set; }
    }
}
