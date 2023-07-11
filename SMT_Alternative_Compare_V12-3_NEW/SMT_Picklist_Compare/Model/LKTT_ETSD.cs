using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Model
{
    public class LKTT_ETSD
    {
        public string dataE { get; set; }
        public string dataL { get; set; }
        public bool checkY { get; set; }

        public LKTT_ETSD()
        {

        }
        public LKTT_ETSD(string dataE, string dataL, bool? checkY)
        {
            this.dataE = dataE;
            this.dataL = dataL;
            this.checkY = checkY.HasValue? checkY.Value: false;//Thuc hien neu co du lieu thi lay con khong co thi la mac dinh la false
        }
        public LKTT_ETSD(string dataE, string dataL, string tempY)
        {
            this.dataE = dataE;
            this.dataL = dataL;
            if (tempY.Contains("Y"))
            {
                this.checkY = true;
            }
            else
            {
                this.checkY = false;
            }

        }
        public LKTT_ETSD(LKTT_ETSD s)
        {
            this.dataE = s.dataE;
            this.dataL = s.dataL;
            this.checkY = s.checkY;
        }
    }
}
