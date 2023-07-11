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
                AddItemMain(ref listDataOut, listFeeder_1, listFeeder_2, itemCurrent);//Add chinh no vao phu hop

                foreach (var itemChild in listChild)
                {
                    //Neu no xuat hien trong listExistEL thì thôi
                    if (listExistEL.Contains(itemChild.dataL))
                    {
                        continue;
                    }

                    AddItem_Child(ref listDataOut, listFeeder_1, listFeeder_2, itemChild.dataL);
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

        private static void AddItemMain(ref List<DataOut> listDataOut, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2, string itemCurrent)
        {
            bool boolFeeder2 = false;
            var checkInFeeder = listFeeder_1.FirstOrDefault(p => p.feederItem == itemCurrent);
            if (checkInFeeder == null)
            {
                checkInFeeder = listFeeder_2.FirstOrDefault(p => p.feederItem == itemCurrent);
                boolFeeder2 = true;
            }
            DataOut s = new DataOut();
            if (checkInFeeder == null)
            {
                s.col_1 = itemCurrent;
                s.address_1 = "NULL";
                s.comment_1 = MdlCommn.EXIST_ALL_EL;
            }
            else //Neu khong xuat hien o 1 thi check xuat hien o 2
            {
                if (boolFeeder2 == false)
                {
                    s.col_1 = itemCurrent;
                    s.address_1 = checkInFeeder.feederAddress;
                    s.comment_1 = MdlCommn.EXIST_ALL_EL;
                }
                else
                {
                    s.col_2 = itemCurrent;
                    s.address_2 = checkInFeeder.feederAddress;
                    s.comment_2 = MdlCommn.EXIST_ALL_EL;
                }
            }
            s.tempMain = MdlCommn.EXIST_ALL_TEMPMAIN;
            listDataOut.Add(new DataOut(s));
        }
        private static void AddItem_Child(ref List<DataOut> listDataOut, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2, string itemCurrent)
        {
            bool boolFeeder2 = false;
            var checkInFeeder = listFeeder_1.FirstOrDefault(p => p.feederItem == itemCurrent);
            if (checkInFeeder == null)
            {
                checkInFeeder = listFeeder_2.FirstOrDefault(p => p.feederItem == itemCurrent);
                boolFeeder2 = true;
            }
            DataOut s = new DataOut();
            if (checkInFeeder != null)
            {
                if (boolFeeder2 == false)
                {
                    s.col_1 = itemCurrent;
                    s.address_1 = checkInFeeder.feederAddress;
                    s.comment_1 = MdlCommn.EXIST_ALL_EL;
                }
                else
                {
                    s.col_2 = itemCurrent;
                    s.address_2 = checkInFeeder.feederAddress;
                    s.comment_2 = MdlCommn.EXIST_ALL_EL;
                }
                s.tempMain = MdlCommn.EXIST_ALL_TEMPMAIN;
                listDataOut.Add(new DataOut(s));
            }
            
        }
    }
}
