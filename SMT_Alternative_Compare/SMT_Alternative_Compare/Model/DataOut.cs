using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Alternative_Compare.Model
{
    public class DataOut
    {
        public string col_1 { get; set; }
        public string col_2 { get; set; }

        public DataOut(string value1, string value2)
        {
            this.col_1 = value1;
            this.col_2 = value2;
        }

    }
    public class DataEL
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public DataEL(string add1, string add2)
        {
            this.address1 = add1;
            this.address2 = add2;
        }
    }
}
