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
        /// <summary>
        /// TH: item xuat hien o 2 vi tri EL(itemEL) va duyet item con được kéo theo bởi itemEL
        /// </summary>
        /// <param name="listDataOut">gia tri tra ve</param>
        /// <param name="listLKTT_ALL">danh sach linh kien thay the sau khi gop</param>
        /// <param name="listFeeder_1">ket qua feeder 1</param>
        /// <param name="listFeeder_2">ket qua feeder 2</param>
        public static void GetAddressEL(ref List<DataOut> listDataOut, List<LKTT_ETSD> listLKTT_ALL, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2)
        {
            //Thuc hien lay cac item xuat hien 2 lan
            List<string> listExistEL = new List<string>();
            GetItemEL(listLKTT_ALL, ref listExistEL);

            //Duyet cac item xuat hien 2 lan
            foreach (var itemCurrent in listExistEL)
            {
                //Lay group thuoc thay the cho E
                var listChild = listLKTT_ALL.Where(x => x.dataE == itemCurrent).ToList();
               
                //Add chinh no neu co address
                Add_EL(ref listDataOut, listFeeder_1, listFeeder_2, itemCurrent);

                //Kiem tra group cua item Current
                foreach (var itemChild in listChild)
                {
                    //Neu no xuat hien trong listExistEL thì thôi
                    if (listExistEL.Contains(itemChild.dataL))
                    {
                        continue;
                    }

                    //Add gia tri con vao EL
                    Add_EL(ref listDataOut, listFeeder_1, listFeeder_2, itemChild.dataL);
                }
            }

        }

        /// <summary>
        /// Thuc hien lay item xuat hien o ca 2 vi tri EL
        /// </summary>
        /// <param name="listLKTT_ALL">List linh kien thay the tong hop cua ca 2 file</param>
        /// <param name="listExistEL">Bien dung de tra ve ket qua xuat hien danh sach cac item EL</param>
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
        /// <summary>
        /// Thuc hien lay gia tri vao listDataOut khi no xuat hien address, nếu address_1, address_2 khong co du lieu thi khong can add
        /// </summary>
        /// <param name="listDataOut">du lieu dau ra</param>
        /// <param name="listFeeder_1">file feeder 1</param>
        /// <param name="listFeeder_2">file feeder 2</param>
        /// <param name="itemCurrent"> item đang can duyet</param>
        private static void Add_EL(ref List<DataOut> listDataOut, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2, string itemCurrent)
        {
            //Kiem tra item trong ItemMain
            DataOut s = new DataOut();
            var checkInFeeder_1 = listFeeder_1.FirstOrDefault(p => p.feederItem == itemCurrent);
            s.col_1 = itemCurrent;
            s.address_1 = checkInFeeder_1 == null ? MdlCommn.NULL_ADDRESS : checkInFeeder_1.feederAddress;
            
            var checkInFeeder_2 = listFeeder_2.FirstOrDefault(p => p.feederItem == itemCurrent);
            s.col_2 = itemCurrent;
            s.address_2 = checkInFeeder_2 == null ? MdlCommn.NULL_ADDRESS : checkInFeeder_2.feederAddress;
           
            //Kiem tra neu 1 trong 2 co du lieu adđress thi add vao list gia tri tra ve
            if (s.address_1 != MdlCommn.NULL_ADDRESS || s.address_2 != MdlCommn.NULL_ADDRESS)
            {
                s.comment_1 = MdlCommn.EXIST_ALL_EL;
                s.comment_2 = MdlCommn.EXIST_ALL_EL;
                s.tempMain = MdlCommn.EXIST_ALL_TEMPMAIN;
                listDataOut.Add(new DataOut(s));
            }

        }
    }
}
