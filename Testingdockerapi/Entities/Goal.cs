using System;
using System.Collections.Generic;
using System.Text;

namespace Testingdockerapi.Entities
{
    public class Goal
    {
        public int id { get; set; }
        public int clientId { get; set; }
        public double amout { get; set; }
        public int startYear { get; set; }
        public int endYear { get; set; }
    }
}
