using System;
using System.Collections.Generic;
using System.Text;

namespace Testingdockerapi.Entities
{
    public class Client
    {
        public string id { get; set; }
        public string name { get; set; }
        public double salary { get; set; }
        public int currentAge { get; set; }
        public int retirementAge { get; set; }
        public Goal goal { get; set; }

    }
}
