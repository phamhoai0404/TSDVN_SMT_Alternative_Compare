using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Model
{
    public class Feeder
    {
        public string feederItem{ get; set; }
        public string  feederAddress { get; set; }
        public Feeder()
        {

        }
        public Feeder(string item, string address)
        {
            this.feederItem = item;
            this.feederAddress = address;
        }
        public Feeder(Feeder s)
        {
            this.feederItem = s.feederItem;
            this.feederAddress = s.feederAddress;
        }
    }
}
