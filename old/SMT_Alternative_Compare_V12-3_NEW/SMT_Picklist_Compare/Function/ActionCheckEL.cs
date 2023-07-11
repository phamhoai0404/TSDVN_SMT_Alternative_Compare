using SMT_Picklist_Compare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Function
{
    class ActionCheckEL
    {
        public static void GetAddressEL(ref List<DataOut> listDataOut, List<LKTT_ETSD> listLKTT_ALL, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2)
        {
            //Thuc hien lay cac item xuat hien 2 lan
            List<string> listExistEL = new List<string>();
            GetItemEL(listLKTT_ALL, ref listExistEL);


            foreach (var itemCurrent in listExistEL)
            {
                //Lay group thuoc thay the cho E
                var listChild = listLKTT_ALL.Where(x => x.dataE == itemCurrent).ToList();
                Add_EL(ref listDataOut, listFeeder_1, listFeeder_2, itemCurrent,  true);//Add chinh no vao phu hop
                foreach (var itemChild in listChild)
                {
                    //Neu no xuat hien trong listExistEL thì thôi
                    if (listExistEL.Contains(itemChild.dataL))
                    {
                        continue;
                    }

                    Add_EL(ref listDataOut, listFeeder_1, listFeeder_2, itemChild.dataL, false);
                }
            }

        }


        /// <summary>
        /// Thuc hien lay item xuat hien o ca 2 vi tri EL
        /// </summary>
        /// <param name="listLKTT_ALL"></param>
        /// <param name="listExistEL"></param>
        private static void GetItemEL(List<LKTT_ETSD> listLKTT_ALL, ref List<string> listExistEL)
        {
            foreach (var itemCurrent in listLKTT_ALL)
            {
                bool checkExist = listExistEL.Contains(itemCurrent.dataE);//Kiem tra xem ton tai trong list chua
                if (checkExist)//Neu xuat hien roi thi duyet item khac
                {
                    continue;
                }

                //Kiem tra xem co ton tai trong cot L hay khong
                bool existL = listLKTT_ALL.Any(x => x.dataL == itemCurrent.dataE);
                if (existL)
                {
                    listExistEL.Add(itemCurrent.dataE);
                }
            }
        }

        private static void Add_EL(ref List<DataOut> listDataOut, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2, string itemCurrent, bool isMain)
        {
            //Kiem tra item trong ItemMain
            DataOut s = new DataOut();
            var checkInFeeder_1 = listFeeder_1.FirstOrDefault(p => p.feederItem == itemCurrent);
            s.col_1 = itemCurrent;
            s.address_1 = checkInFeeder_1 == null ? MdlCommn.NULL_ADDRESS : checkInFeeder_1.feederAddress;
            s.comment_1 = MdlCommn.EXIST_ALL_EL;

            var checkInFeeder_2 = listFeeder_2.FirstOrDefault(p => p.feederItem == itemCurrent);
            s.col_2 = itemCurrent;
            s.address_2 = checkInFeeder_2 == null ? MdlCommn.NULL_ADDRESS : checkInFeeder_2.feederAddress;
            s.comment_2 = MdlCommn.EXIST_ALL_EL;

            s.tempMain = MdlCommn.EXIST_ALL_TEMPMAIN;

            if (isMain)
            {
                listDataOut.Add(new DataOut(s));
            }
            else
            {
                if(s.address_1 != MdlCommn.NULL_ADDRESS || s.address_2 != MdlCommn.NULL_ADDRESS)
                {
                    listDataOut.Add(new DataOut(s));
                }
            }
        }
    }
}
