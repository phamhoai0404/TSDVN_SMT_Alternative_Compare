using SMT_Picklist_Compare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Function
{
    public class ActionMain
    {
        public static void ActionCompareETSD(List<Picklist> picklist_1, List<Picklist> picklist_2, List<LKTT_ETSD> listLKTT, ref List<DataOut> listResult)
        {
            foreach (Picklist itemList_1 in picklist_1)
            {
                bool existAll = false;
                bool existE = listLKTT.Any(x => x.dataE == itemList_1.plItem);
                bool existL = listLKTT.Any(x => x.dataL == itemList_1.plItem);
                if(existE == true && existL == true)
                {
                    existAll = true;
                }

                //Neu no ton tai trong ca 2 thi them ca 2
                if (picklist_2.Any(x => x.plItem == itemList_1.plItem))
                {
                    listResult.Add(new DataOut(itemList_1.plItem, itemList_1.plItem, existAll));
                }

                GetDataWhenE(ref listResult, listLKTT, picklist_2, itemList_1.plItem, existAll);
                GetDataWhenL(ref listResult, listLKTT, picklist_2, itemList_1.plItem, existAll);
            }
            RemoveDuplicate(ref listResult);
            RemoveNotIn(ref listResult);
        }


        private static void GetDataWhenE(ref List<DataOut> listData, List<LKTT_ETSD> listLKTT, List<Picklist> list2, string itemCurrent, bool existAll)
        {
            //Thuc hien dang thuoc truong hop nay
            //E-Y
            //E-Y1
            //E-Y2 => Thuc hien kiem tra Y, Y1, Y2 => xem co ton tai trong List 2 hay khong roi lay vao
            var listWhenE = listLKTT.Where(p => p.dataE == itemCurrent).ToList();
            foreach (var item in listWhenE)
            {
                bool checkExist = list2.Any(p => p.plItem == item.dataL);
                if (checkExist)
                {
                    listData.Add(new DataOut(item.dataE, item.dataL, existAll));
                }
            }
        }
        private static void GetDataWhenL(ref List<DataOut> listData, List<LKTT_ETSD> listLKTT, List<Picklist> list2, string itemCurrent, bool existAll)
        {
            //Thuc hien dang thuoc truong hop nay
            //E1-Y
            //E2-Y
            //E1-Z
            //E1-N
            //E2-K
            // Thuc hien kiem tra xem E1, E2, Z,N,K => xem co ton tai trong List 2 hay khong roi lay vao


            //Thuc hien lay du lieu cua LKTT khi no nam = L
            var listWhenL = listLKTT.Where(p => p.dataL == itemCurrent).ToList();
            
            //Kiem tra cac cap link kien vua lay duoc => tung con 1 va duyet E cua no
            foreach (var item in listWhenL)
            {
                //Thuc hien kiem tra xem co ton tai trong gia tri E co nam trong picklist hay khong => co add vao
                bool tempCheck = list2.Any(p => p.plItem == item.dataE);
                if(tempCheck)
                {
                    listData.Add(new DataOut(itemCurrent, item.dataE, existAll));
                } 

                //Thuc lay nhom theo E
                var listWhenE = listLKTT.Where(p => p.dataE == item.dataE).ToList();
                
                //Kiem tra trong su rang buoc
                foreach (var itemRef in listWhenE)
                {
                    //Kiem tra su ton tai cua no trong picklist 2
                    bool checkExist = list2.Any(p => p.plItem == itemRef.dataL);
                    if (checkExist)
                    {
                        listData.Add(new DataOut(itemCurrent, itemRef.dataL, item.dataE, existAll));
                    }
                }
            }
        }


        /// <summary>
        /// Xoa bo cac gia tri trung lap trong list
        /// </summary>
        /// <param name="listDataOut"></param>
        private static void RemoveDuplicate(ref List<DataOut> listDataOut)
        {
            foreach (var item in listDataOut.ToList())
            {
                var checkExistItem = listDataOut.Where(p => p.col_1 == item.col_1 && p.col_2 == item.col_2).Count();
                if(checkExistItem > 1)
                {
                    listDataOut.Remove(item);
                }
            }
        }

        /// <summary>
        /// Thuc hien xoa bo khi col1 == col2 ma no khong ton ton tai trong cac dong du lieu khac
        /// </summary>
        /// <param name="listDataOut"></param>
        private static void RemoveNotIn(ref List<DataOut> listDataOut)
        {
            foreach (var item in listDataOut.ToList())
            {
                if(item.col_1 == item.col_2)//Neu ma bang
                {
                    var check_Col1 = listDataOut.Where(p => p.col_1 == item.col_1 || p.col_2 == item.col_1 || p.tempMain == item.col_1).Count();
                    if(check_Col1 <= 1)
                    {
                        listDataOut.Remove(item);
                    }
                }

            }
        }
    }

}
