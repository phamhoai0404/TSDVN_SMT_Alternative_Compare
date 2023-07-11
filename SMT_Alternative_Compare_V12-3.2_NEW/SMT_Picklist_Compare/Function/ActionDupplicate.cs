using SMT_Picklist_Compare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Function
{
    public class ActionDupplicate
    {
        public static void RemoveDupplicate(ref List<DataOut> listDataOut)
        {
            foreach (var itemCurrent in listDataOut.ToList())
            {
                //Neu 1 trong 2 address khong co dia chi thi khong duyet item nay nua
                if(itemCurrent.address_1 == null || itemCurrent.address_2 == null)
                {
                    continue;
                }

                //Truong hop neu 1 trong 2 ma khac gia tri null thi dung lai
                if(itemCurrent.address_1 != MdlCommn.NULL_ADDRESS || itemCurrent.address_2 != MdlCommn.NULL_ADDRESS)
                {
                    continue;
                }

                //Truong hop ca 2 khong co du lieu thi thuc hien xoa bo
                if(string.IsNullOrEmpty(itemCurrent.comment_1) && string.IsNullOrEmpty(itemCurrent.comment_2))
                {
                    listDataOut.Remove(itemCurrent);
                }
            }
        }
    }
}
