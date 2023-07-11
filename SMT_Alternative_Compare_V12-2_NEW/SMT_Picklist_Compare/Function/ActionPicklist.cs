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

        public static void AddComment(ref List<DataOut> listDataOut, 
                                        List<Picklist> picklist_1, List<Picklist> picklist_2, 
                                        List<LKTT_ETSD> listLKTT_1, List<LKTT_ETSD> listLKTT_2)
        {
            //Thuc hien lay ra cac comment cua item
            var listComment_1 = picklist_1.Where(p => string.IsNullOrEmpty(p.plComment) == false).ToList();
            foreach (var itemLK in listComment_1)
            {
                bool checkInList = false;//Bien co
                foreach (var itemCurrent in listDataOut)
                {
                    if(itemCurrent.col_1 == itemLK.plItem)//Neu item gion nhau
                    {
                        itemCurrent.comment_1 += itemLK.plComment;
                        checkInList = true;
                    }
                }
                if (checkInList == false)//Xet truong hop chua co trong listResult
                {
                    //MAI CAN PHAI LAM O DAU
                    
                    DataOut value = new DataOut();//Thuc hien add comment moi
                    value.col_1 = itemLK.plItem;
                    value.comment_1 = itemLK.plComment;
                    listDataOut.Add(value);
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
                    DataOut value = new DataOut();
                    value.col_2 = itemLK.plItem;
                    value.comment_2 = itemLK.plComment;
                    listDataOut.Add(value);
                }
            }

            //Duyet du lieu xem co check Y hay khong
            //Phan nay can xem lai
            foreach (var item in listDataOut)
            {
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
    }
}
