using SMT_Picklist_Compare.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Function
{
    class ActionPicklist
    {
        /// <summary>
        /// Thuc hien lay du lieu tu file CSV
        /// </summary>
        /// <param name="pathFile"></param>
        /// <param name="listPicklist"></param>
        /// <param name="lblWO"></param>
        public static void GetDataCSV(string pathFile, ref List<Picklist> listPicklist, ref System.Windows.Forms.Label lblWO)
        {
            try
            {
                string tempItem = "";
                string tempComment = "";
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                        for (int i = 1; i < lines.Count() - 1; i++)
                        {
                            var delimitedLine = CSVParser.Split(lines[i]);
                            tempItem = delimitedLine[Model.ConstSelectFile.INDEX_GET_FILE_12]?.Replace("\"", "").Trim();
                            if (tempItem?.Length < 15)
                            {
                                continue;
                            }

                            var itemExist = listPicklist.FirstOrDefault(p => string.Equals(p.plItem, tempItem, StringComparison.OrdinalIgnoreCase));
                            tempComment = delimitedLine[Model.ConstSelectFile.INDEX_GET_COMMENT]?.Replace("\"", "").Trim();
                            tempItem = tempItem.ToUpper();
                            if (itemExist == null)
                            {
                                listPicklist.Add(new Picklist(tempItem, tempComment));
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(tempComment) == false)//Neu khong co du lieu comment thi thoi con co du lieu thi thuc hien thay doi comment
                                {
                                    //Thay vi su dung foreach ta co the dung tim ra vi tri roi thay doi gia tri cua comment

                                    foreach (var picklist in listPicklist)
                                    {
                                        if (picklist.plItem == tempItem)
                                        {
                                            picklist.plComment = tempComment;
                                            break;
                                        }
                                    } //Neu co comment thi thuc hien gan comment mo
                                }
                            }
                        }

                        if (lines.Count() >= 1)
                        {
                            lblWO.Text = CSVParser.Split(lines[1])[Model.ConstSelectFile.INDEX_GET_NAME_WO].Replace("\"", "");
                            lblWO.Text += " : " + CSVParser.Split(lines[1])[Model.ConstSelectFile.INDEX_GET_NAME_MODEL].Replace("\"", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ActionPicklist: GetDataCSV - PathFile - {pathFile}: {ex.Message}");
            }
        }

        /// <summary>
        /// Thuc hien lay du lieu cua picklist sau khi xoa bo nhung con trung lap
        /// </summary>
        /// <param name="list_1"></param>
        /// <param name="list_2"></param>
        /// <param name="listLKTT_All"></param>
        /// <param name="pickListAfter_1"></param>
        /// <param name="pickListAfter_2"></param>
        public static void CompareTwoFile(List<Picklist> list_1, List<Picklist> list_2, List<LKTT_ETSD> listLKTT_All,
                                            ref List<Picklist> pickListAfter_1, ref List<Picklist> pickListAfter_2)
        {
            try
            {
                pickListAfter_1 = list_1.ToList();
                pickListAfter_2 = list_2.ToList();

                foreach (var row in pickListAfter_1.ToList())
                {
                    int indexIn2 = pickListAfter_2.FindIndex(x => x.plItem == row.plItem);
                    if (indexIn2 != -1)//Neu ma ton tai trong 2
                    {
                        var checkObject = listLKTT_All.Any(x => x.dataE == row.plItem || x.dataL == row.plItem);//Kiem tra 
                        if (checkObject == false)
                        {
                            pickListAfter_1.Remove(row);
                            pickListAfter_2.RemoveAt(indexIn2);//Xoa vi tri trong list2
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ActionPicklist: CompareTwoFile: {ex.Message}");
            }
        }

        /// <summary>
        /// Thuc hien add them comment của chuong trinh
        /// </summary>
        /// <param name="listDataOut"></param>
        /// <param name="picklist_1"></param>
        /// <param name="picklist_2"></param>
        /// <param name="listLKTT_1"></param>
        /// <param name="listLKTT_2"></param>
        public static void AddComment(ref List<DataOut> listDataOut, 
                                        List<Picklist> picklist_1, List<Picklist> picklist_2, 
                                        List<LKTT_ETSD> listLKTT_1, List<LKTT_ETSD> listLKTT_2)
        {
            //Add comment va duyet cac comment chua duoc ghep vao cac dong cua tung Picklist
            List<PicklistType> listPicklistType = new List<PicklistType>();
            AddCommentNormal(ref listDataOut, picklist_1, picklist_2, ref listPicklistType);

            //Add comment o truong hop khac
            AddCommentOther(ref listDataOut, listPicklistType);

            //Add Check Y voi truong hop EL thi thoi
            AddCheckY(ref listDataOut, listLKTT_1, listLKTT_2);
            
        }


        private static void AddCheckY (ref List<DataOut> listDataOut, List<LKTT_ETSD> listLKTT_1, List<LKTT_ETSD> listLKTT_2)
        {
            //Duyet du lieu xem co check Y hay khong
            //Phan nay can xem lai
            foreach (var item in listDataOut)
            {
                //No thuoc truong hop duyet sau cung EL thi thoi
                if (item.tempMain != null)
                {
                    if (item.tempMain == MdlCommn.EXIST_ALL_TEMPMAIN)
                    {
                        continue;
                    }
                }


                //Duyet truong hop tich Y thong thuong
                var checkOne_Y = listLKTT_1.Any(p => p.checkY == true && p.dataL == item.col_1);
                if (checkOne_Y)
                {
                    item.comment_1 = MdlCommn.CHECKY + item.comment_1;
                }

                var checkTwo_Y = listLKTT_2.Any(p => p.checkY == true && p.dataL == item.col_2);
                if (checkTwo_Y)
                {
                    item.comment_2 = MdlCommn.CHECKY + item.comment_2;//Neu la tich Y thi cong them CheckY
                }
            }
        }

        

        private static void AddCommentNormal( ref List<DataOut> listDataOut, List<Picklist> picklist_1, List<Picklist> picklist_2, ref List<PicklistType> listPicklistType)
        {
            //Thuc hien lay ra cac comment cua item
            var listComment_1 = picklist_1.Where(p => string.IsNullOrEmpty(p.plComment) == false).ToList();
            foreach (var itemLK in listComment_1)
            {
                bool checkInList = false;//Bien co
                foreach (var itemCurrent in listDataOut)
                {
                    if (itemCurrent.col_1 == itemLK.plItem)//Neu item gion nhau
                    {
                        itemCurrent.comment_1 += itemLK.plComment;
                        checkInList = true;
                    }
                }
                if (checkInList == false)//Xet truong hop chua co trong listResult
                {
                    listPicklistType.Add(new PicklistType(itemLK, true));//Truong hop commment thuoc picklist_1
                }
            }

            //Phan 2 tuong tu nhu phan 1
            var listComment_2 = picklist_2.Where(p => string.IsNullOrEmpty(p.plComment) == false).ToList();
            foreach (var itemLK in listComment_2)
            {
                bool checkInList = false;
                foreach (var itemCurrent in listDataOut)
                {
                    if (itemCurrent.col_2 == itemLK.plItem)//Neu item gion nhau
                    {
                        itemCurrent.comment_2 += itemLK.plComment;
                        checkInList = true;
                    }
                }
                if (checkInList == false)
                {
                   listPicklistType.Add(new PicklistType(itemLK, false));//Truong hop comment thuoc picklist_2
                }
            }
        }

        /// <summary>
        /// Thuc hien add comment o item chua trong listDataOut theo truong hop binh thuong
        /// </summary>
        /// <param name="listDataOut"></param>
        /// <param name="listPicklistType">Danh sach cac item chua duoc duyet comment </param>
        private static void AddCommentOther(ref List<DataOut> listDataOut, List<PicklistType> listPicklistType)
        {
            foreach (var itemCurrent in listPicklistType)
            {
                bool checkAdd = false;//Bien tam dung de kiem tra xem da add du lieu hay chua
                foreach (var itemOut in listDataOut)
                {
                    if (itemOut.tempMain == null)
                    {
                        continue;
                    }//Neu null thi bo qua
                    if (itemOut.tempMain != MdlCommn.EXIST_ALL_TEMPMAIN)
                    {
                        continue;
                    }//Neu khong phai lam tempMain EL thi bo qua

                    //Kiem tra xem 1 co =?
                    if (itemOut.col_1 == itemCurrent.plItem)
                    {
                        itemOut.comment_1 += itemCurrent.plComment;
                        checkAdd = true;
                    }

                    //Kiem tra xem 2 co =?
                    if (itemOut.col_2 == itemCurrent.plItem)
                    {
                        itemOut.comment_1 += itemCurrent.plComment;
                        checkAdd = true;
                    }
                }

                //Trong truong hop comment chua duoc add vao thi can phai add them comment vao voi item la moi
                if (checkAdd == false)
                {
                    DataOut s = new DataOut();
                    if (itemCurrent.isFirst)
                    {
                        s.col_1 = itemCurrent.plItem;
                        s.comment_1 = itemCurrent.plComment;
                    }
                    else
                    {
                        s.col_2 = itemCurrent.plItem;
                        s.comment_2 = itemCurrent.plComment;
                    }
                    listDataOut.Add(s);
                }
            }
        }
    }
}
