using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Testingdockerapi.Entities
{
    
    public class Cashflow
    {
        public string id { get; set; }
        public string ClientId { get; set; }
        public string type { get; set; }
        public double amount { get; set; }

        public string name { get; set; }
    }
}
