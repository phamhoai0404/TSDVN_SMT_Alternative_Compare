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
    public class MyFunction
    {
        #region Select File And Check Input
        /// <summary>
        /// Thuc hien select file 
        /// </summary>
        /// <returns>
        /// Tra ve ket qua la dia chi file; hoac khong chon file nao; hoac nhay vao catch
        /// </returns>
        /// CreatedBy: HoaiPT(01/02/2023)
        public static string SelectFile()
        {
            try
            {
                using (var ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Filter = Model.ConstSelectFile.TYPE_FILE;
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        return ofd.FileName;
                    }
                    return Model.ResultSelectFile.NOT_SELECT_FILE;
                }
            }
            catch (Exception)
            {
                return Model.ResultSelectFile.TO_CATCH;
            }


        }

        /// <summary>
        /// Thuc hien kiem tra du lieu dau vao
        /// </summary>
        /// <param name="valueInput"></param>
        /// <returns></returns>
        public static string CheckInput(ref Input valueInput)
        {
            try
            {
                //Check dau vao khong duoc de trong
                if (string.IsNullOrWhiteSpace(valueInput.file_1) ||
                   string.IsNullOrWhiteSpace(valueInput.file_2) ||
                   string.IsNullOrWhiteSpace(valueInput.file_ETSD1) ||
                   string.IsNullOrWhiteSpace(valueInput.file_ETSD2) ||
                   string.IsNullOrWhiteSpace(valueInput.fileLinkData1) ||
                   string.IsNullOrWhiteSpace(valueInput.fileLinkData2))
                {
                    return Model.ResultCheckInput.ERROR_EMPTY;
                }

                //Thuc hien bo di cac ki tu thua neu co
                valueInput.file_1 = valueInput.file_1.Trim();
                valueInput.file_2 = valueInput.file_2.Trim();
                valueInput.file_ETSD1 = valueInput.file_ETSD1.Trim();
                valueInput.file_ETSD2 = valueInput.file_ETSD2.Trim();
                valueInput.fileLinkData1 = valueInput.fileLinkData1.Trim();
                valueInput.fileLinkData2 = valueInput.fileLinkData2.Trim();


                //Kiem tra 2 file dau vao co trung nhau hay khong
                if (valueInput.file_1.Equals(valueInput.file_2) ||
                    valueInput.file_1.Equals(valueInput.file_ETSD1) ||
                    valueInput.file_1.Equals(valueInput.file_ETSD2) ||
                    valueInput.file_1.Equals(valueInput.fileLinkData1) ||
                    valueInput.file_1.Equals(valueInput.fileLinkData2) ||
                    valueInput.file_2.Equals(valueInput.file_ETSD1) ||
                    valueInput.file_2.Equals(valueInput.file_ETSD2) ||
                    valueInput.file_2.Equals(valueInput.fileLinkData1) ||
                    valueInput.file_2.Equals(valueInput.fileLinkData2) ||
                    valueInput.file_ETSD1.Equals(valueInput.fileLinkData1) ||
                    valueInput.file_ETSD1.Equals(valueInput.fileLinkData2) ||
                    valueInput.fileLinkData1.Equals(valueInput.file_ETSD2) ||
                    valueInput.fileLinkData2.Equals(valueInput.file_ETSD2))
                {
                    return Model.ResultCheckInput.ERROR_DUPLICATE_FILE;
                }

                if (!File.Exists(valueInput.file_1))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_1;
                }
                if (!File.Exists(valueInput.file_2))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_2;
                }
                if (!File.Exists(valueInput.file_ETSD1))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_ETSD_1;
                }
                if (!File.Exists(valueInput.file_ETSD2))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_ETSD_2;
                }
                if (!File.Exists(valueInput.fileLinkData1))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_LINK_DATA_1;
                }
                if (!File.Exists(valueInput.fileLinkData2))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_LINK_DATA_2;
                }

                if (CheckFileCSV(valueInput.file_1) == false ||
                    CheckFileCSV(valueInput.file_2) == false ||
                    CheckFileCSV(valueInput.fileLinkData1) == false ||
                    CheckFileCSV(valueInput.fileLinkData2) == false ||
                    CheckFileCSV(valueInput.file_ETSD1) == false ||
                    CheckFileCSV(valueInput.file_ETSD2) == false
                    )
                {
                    return Model.ResultCheckInput.ERROR_NOT_CSV;
                }
                return Model.ResultCheckInput.OK;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static bool CheckFileCSV(string pathFile)
        {
            try
            {
                FileInfo file = new FileInfo(pathFile);
                if (file.Extension.Equals(".csv"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion


        /// <summary>
        /// Thuc hien lay du lieu cac item cua file
        /// </summary>
        /// <param name="pathFile">Dia chi file</param>
        /// <param name="indexColumn">Dia chi cot du lieu muon lay</param>
        /// <param name="hasHeader">Ton  tai tieu de trong du lieu hay khong</param>
        /// <param name="nameWO">Thuc hien lay ten cua WO hay khong? neu nameWO == loai lay ten ==> tra ve ten WO</param>
        /// <returns>
        /// Tra ve danh sach Item lay duoc
        /// </returns>
        /// CreatedBy: HoaiPT(02/02/2023)
        public static List<string> GetDataItemFile(string pathFile, int indexColumn, ref string nameWO, bool hasHeader = true)
        {
            try
            {
                //Bien lay du lieu
                int indexStart = 0;
                if (hasHeader)
                {
                    indexStart = 1;//Neu hasHeader = true => Co tieu de se bo di tieu de
                }

                List<string> listFile1 = new List<string>();
                string temp = "";
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                        for (int i = indexStart; i < lines.Count() - 1; i++)
                        {
                            var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab
                            temp = delimitedLine[indexColumn].Replace("\"", "");
                            listFile1.Add(temp);
                        }

                        if (nameWO.Equals(Model.ConstSelectFile.TYPE_GET_NAME_WO))
                        {
                            nameWO = CSVParser.Split(lines[indexStart])[Model.ConstSelectFile.INDEX_GET_NAME_WO].Replace("\"", "");
                            nameWO += " : " + CSVParser.Split(lines[indexStart])[Model.ConstSelectFile.INDEX_GET_NAME_MODEL].Replace("\"", "");
                        }

                    }
                }
                return listFile1;
            }
            catch (Exception)
            {
                return null;
            }
        }



        /// <summary>
        /// Thuc hien so sanh du lieu cua 2 list va xoa bo di du lieu giong nhau
        /// Va gia tri giong nhau do khong ton tai trong cot E va L cua file ETSD
        /// </summary>
        /// <param name="listFile1">Du lieu file 1</param>
        /// <param name="listFile2">Du lieu file 2</param>
        /// <param name="listETSD_E">Du lieu item Cot E</param>
        /// <param name="listETSD_L">Du lieu item Cot L</param>
        /// <returns></returns>
        /// UpdateBy: HoaiPT(04/02/2023)
        public static string CompareTwoFile(ref List<string> listFile1, ref List<string> listFile2, List<string> listETSD_E, List<string> listETSD_L)
        {
            try
            {
                foreach (var item in listFile1.ToList())
                {
                    bool checkExistItem = listFile2.Contains(item);
                    if (checkExistItem)
                    {
                        checkExistItem = listETSD_E.Contains(item);
                        if (checkExistItem == false)
                        {
                            checkExistItem = listETSD_L.Contains(item);
                            if (checkExistItem == false)
                            {
                                listFile1.Remove(item);
                                listFile2.Remove(item);
                            }
                        }


                    }
                }
                if (listFile1.Count == 0 || listFile2.Count == 0)
                {
                    return Model.ResultCompareTwoFile.INFOR_EMPTY;
                }

                return Model.ResultCompareTwoFile.OK;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Thuc hien hanh dong so sanh lay ra cac cap gia tri phu hop
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="listETSD_E"></param>
        /// <param name="listETSD_L"></param>
        /// <param name="listResult"></param>
        /// <returns></returns>
        public static string ActionCompareETSD(List<string> list1, List<string> list2, List<string> listETSD_E, List<string> listETSD_L, ref List<DataOut> listResult)
        {
            try
            {
                foreach (var item in list1.ToList())
                {
                    List<DataEL> tempDataEL = new List<DataEL>();
                    bool tempValueIn_E = true;

                    if (list2.Contains(item))
                    {
                        listResult.Add(new DataOut(item, item));
                    }

                    //Duyet cot E
                    for (int i = 0; i < listETSD_E.Count; i++)
                    {
                        if (item.Equals(listETSD_E[i]))
                        {
                            tempDataEL.Add(new DataEL(listETSD_E[i], listETSD_L[i]));
                        }
                    }

                    //Bat dau chuyen sang cot L
                    if (tempDataEL.Count == 0)
                    {
                        for (int i = 0; i < listETSD_L.Count; i++)
                        {
                            if (item.Equals(listETSD_L[i]))
                            {
                                tempDataEL.Add(new DataEL(listETSD_L[i], listETSD_E[i]));
                            }
                        }
                        tempValueIn_E = false;
                    }

                    //Neu khong co du lieu thi dung lai ma thoi
                    if (tempDataEL.Count == 0)
                        continue;

                    if (tempValueIn_E == true)
                    {
                        //Truong hop co du lieu o phan cot E
                        foreach (var itemChild in tempDataEL)
                        {
                            bool existChild = list2.Contains(itemChild.address2);
                            if (existChild)
                            {
                                listResult.Add(new DataOut(item, itemChild.address2));
                                list2.Remove(itemChild.address2);

                            }
                        }
                    }
                    else
                    {
                        //Truong hop co du lieu o phan cot L
                        string tempAl_L = tempDataEL[0].address2;
                        tempDataEL.Clear();//Xoa bo cu
                        for (int i = 0; i < listETSD_E.Count; i++)//Thuc hien loc ra list chua giong
                        {
                            if (listETSD_E[i].Equals(tempAl_L))
                            {
                                tempDataEL.Add(new DataEL(listETSD_E[i], listETSD_L[i]));
                            }
                        }

                        bool tempTempTemp = true;//Bien kiem tra neu ton tai du lieu 
                        foreach (var itemChild in tempDataEL)
                        {
                            bool existChild = list2.Contains(itemChild.address2);
                            if (existChild)
                            {
                                if (itemChild.address2 != item)
                                {
                                    listResult.Add(new DataOut(item, itemChild.address2, tempAl_L));
                                }
                                tempTempTemp = false;
                            }
                        }
                        if (list2.Contains(tempAl_L))//Phan thuc hien neu chua A thi add A - A
                        {
                            listResult.Add(new DataOut(item, tempAl_L));
                        }

                        if (tempTempTemp == true)
                        {
                            foreach (var itemChild in tempDataEL)
                            {
                                bool tempCheck = listResult.Any(x => x.col_2 == itemChild.address2 && x.col_2 != item);
                                if (tempCheck == true)
                                {
                                    listResult.Add(new DataOut(item, itemChild.address2, tempAl_L));
                                }
                            }
                        }
                    }
                }

                if (listResult.Count == 0)
                {
                    return Model.ResultCompareETSD.INFOR_EMPTY;
                }
                return Model.ResultCompareETSD.OK;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Thuc hien hanh dong add Address
        /// </summary>
        /// <param name="listDataOut"></param>
        /// <param name="pathFile1"></param>
        /// <param name="pathFile2"></param>
        /// <returns></returns>
        /// CreatedBy: HoaiPT(???)
        /// UpdateBy: HoaiPT(06/02/2023)
        public static string Add_Address(ref List<DataOut> listDataOut, string pathFile1, string pathFile2, List<string> listE, List<string> listL)
        {
            try
            {
                //Thuc hien lay du lieu cua link address 1
                List<DataLink> listLink1 = new List<DataLink>();
                GetDataLinkAddress(ref listLink1, pathFile1);
                if (listLink1.Count == 0)
                {
                    return Model.ResultAddAddress.ERROR_GET_LINK_1;
                }

                //Thuc hien lay du lieu cua link address 2
                List<DataLink> listLink2 = new List<DataLink>();
                GetDataLinkAddress(ref listLink2, pathFile2);
                if (listLink2.Count == 0)
                {
                    return Model.ResultAddAddress.ERROR_GET_LINK_2;
                }

                foreach (var itemData in listDataOut)
                {
                    if (string.IsNullOrWhiteSpace(itemData.tempMain) == false)
                    {
                        List<string> listAfter = new List<string>();//Group cua theo nhom
                        for (int i = 0; i < listE.Count; i++)
                        {
                            if (itemData.tempMain.Equals(listE[i]))
                            {
                                listAfter.Add(listL[i]);
                            }
                        }
                        listAfter.Add(itemData.tempMain);

                        foreach (var item in listAfter)
                        {
                            var listObject = listLink1.Find(x => x.itemName == item);
                            if (listObject != null)
                            {
                                itemData.address_1 = listObject.address;
                                break;
                            }
                        }
                        if (itemData.address_1 == null)
                        {
                            itemData.address_1 = "Khong co 1";
                        }

                        foreach (var item in listAfter)
                        {
                            var listObject = listLink2.Find(x => x.itemName == item);
                            if (listObject != null)
                            {
                                itemData.address_2 = listObject.address;
                                break;
                            }
                        }
                        if (itemData.address_2 == null)
                        {
                            itemData.address_2 = "Khong co 2";
                        }

                    }
                    else
                    {
                        var listObject = listLink1.Find(x => x.itemName == itemData.col_1);
                        if (listObject != null)
                        {
                            itemData.address_1 = listObject.address;
                        }
                        else
                        {
                            listObject = listLink1.Find(x => x.itemName == itemData.col_2);
                            if (listObject != null)
                            {
                                itemData.address_1 = listObject.address;
                            }
                            else
                            {
                                itemData.address_1 = "null";
                            }
                        }

                        listObject = listLink2.Find(x => x.itemName == itemData.col_2);
                        if (listObject != null)
                        {
                            itemData.address_2 = listObject.address;
                        }
                        else
                        {
                            listObject = listLink2.Find(x => x.itemName == itemData.col_1);
                            if (listObject != null)
                            {
                                itemData.address_2 = listObject.address;
                            }
                            else
                            {
                                itemData.address_2 = "null";
                            }
                        }
                    }
                }

                return Model.ResultAddAddress.OK;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Thuc hien lay du lieu o trong file link data 
        /// </summary>
        /// <param name="listLink1"></param>
        /// <param name="pathFile1"></param>
        private static void GetDataLinkAddress(ref List<DataLink> listLink1, string pathFile1)
        {
            try
            {
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                using (FileStream fs = new FileStream(pathFile1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                        for (int i = 1; i < lines.Count() - 1; i++)
                        {
                            var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab
                            listLink1.Add(new DataLink(delimitedLine[Model.ConstDataLink.INDEX_GET_ADDRESS],
                                                       delimitedLine[Model.ConstDataLink.INDEX_GET_ITEM]));
                        }
                    }
                }
            }
            catch (Exception)
            {
                listLink1 = null;
            }
        }

        private static void RemoveDupplicate(ref List<string> list1, ref List<string> list2)
        {
            List<DataEL> listTemp = new List<DataEL>();
            for (int i = 0; i < list1.Count; i++)
            {
                listTemp.Add(new DataEL(list1[i], list2[i]));
            }

            foreach (var item in listTemp.ToList())
            {
                var check = listTemp.Where(x => x.address1.Equals(item.address1) && x.address2.Equals(item.address2)).ToList();
                if (check.Count > 1)
                {
                    listTemp.Remove(item);
                }
            }
            list1.Clear();
            list2.Clear();

            foreach (var item in listTemp)
            {
                list1.Add(item.address1);
                list2.Add(item.address2);
            }
        }

        public static string ActionFineRemove(ref List<DataOut> listResult)
        {
            try
            {
                RemoveCrossPair(ref listResult);
                //Xoa bo di gia tri trung lap neu la A -A ma no khong dinh toi cac dong khac chi ton tai khong nhu nay thi xoa di
                foreach (var itemOut in listResult.ToList())
                {
                    if (itemOut.col_1.Equals(itemOut.col_2))
                    {
                        var listChild = listResult.Where(x => x.col_1.Equals(itemOut.col_2) || x.col_2.Equals(itemOut.col_2)).ToList();
                        if (listChild.Count() == 1)
                        {
                            listResult.Remove(itemOut);
                        }
                    }
                }
                if (listResult.Count == 0)
                {
                    return Model.ResultActionFineRemove.INFOR_EMPTY;
                }
                return Model.ResultActionFineRemove.OK;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Thuc hien xoa khi co doi xung
        /// </summary>
        /// <param name="listDataOut"></param>
        /// CreatedBy: HoaiPT(06/02/2023)
        private static void RemoveCrossPair(ref List<DataOut> listDataOut)
        {
            for (int i = 0; i < listDataOut.Count() - 1; i++)
            {
                for (int j = i + 1; j < listDataOut.Count(); j++)
                {
                    if (listDataOut[i].col_1.Equals(listDataOut[j].col_2) &&
                       listDataOut[i].col_2.Equals(listDataOut[j].col_1) &&
                       listDataOut[i].address_1.Equals(listDataOut[j].address_2) &&
                       listDataOut[i].address_2.Equals(listDataOut[j].address_1))
                    {
                        listDataOut.Remove(listDataOut[j]);
                        listDataOut.Remove(listDataOut[i]);
                        i--;
                    }
                }
            }
        }

        public static void AddComment(ref List<DataOut> listDataOut, Input valueInput)
        {
            List<DataComment> listComment1 = new List<DataComment>();
            List<DataComment> listComment2 = new List<DataComment>();
            GetComment(ref listComment1, valueInput.file_1.Trim());
            GetComment(ref listComment2, valueInput.file_2.Trim());

            foreach (var item in listDataOut)
            {
                var tempObject = listComment1.Where(x => x.item.Equals(item.col_1)).ToList();
                if (tempObject != null)
                {
                    if (tempObject.Count > 0)
                    {
                        item.comment_1 = tempObject[0].comment;
                    }
                }

                tempObject = listComment2.Where(x => x.item.Equals(item.col_2)).ToList();
                if (tempObject != null)
                {
                    if (tempObject.Count > 0)
                    {
                        item.comment_2 = tempObject[0].comment;
                    }

                }
            }

        }
        public static void AddCommentOther(ref List<DataOut> listDataOut, Input valueInput)
        {
            List<DataComment> listComment1 = new List<DataComment>();
            List<DataComment> listComment2 = new List<DataComment>();
            GetComment(ref listComment1, valueInput.file_1.Trim());
            GetComment(ref listComment2, valueInput.file_2.Trim());

            bool check1 = false;
            bool check2 = false;
            foreach (var itemComment1 in listComment1)
            {
                if (!string.IsNullOrWhiteSpace(itemComment1.comment?.Replace("\"", "")))
                {
                    if (checkExistlistDataOut(listDataOut, itemComment1.item, true) == false)
                    {
                        listDataOut.Add(new DataOut(itemComment1.item, itemComment1.comment, 1));
                        check1 = true;
                    }
                }
            }
            foreach (var itemComment2 in listComment2)
            {
                if (!string.IsNullOrWhiteSpace(itemComment2.comment.Replace("\"", "")))
                {
                    if (checkExistlistDataOut(listDataOut, itemComment2.item, false) == false)
                    {
                        listDataOut.Add(new DataOut(itemComment2.item, itemComment2.comment, 2));
                        check2 = true;
                    }
                }
            }
            if (check1 == true)
            {
                //Thuc hien lay du lieu cua link address 1
                List<DataLink> listLink1 = new List<DataLink>();
                GetDataLinkAddress(ref listLink1, valueInput.fileLinkData1);
                foreach (var itemIn in listDataOut)
                {
                    if (itemIn.address_1 == "OTHER")
                    {
                        var listObject = listLink1.Find(x => x.itemName.Contains(itemIn.col_1));
                        if (listObject != null)
                        {
                            itemIn.address_1 = listObject.address;
                        }
                    }
                }
            }
            if (check2 == true)
            {
                //Thuc hien lay du lieu cua link address 2
                List<DataLink> listLink2 = new List<DataLink>();
                GetDataLinkAddress(ref listLink2, valueInput.fileLinkData2);
                foreach (var itemIn in listDataOut)
                {
                    if (itemIn.address_2 == "OTHER")
                    {
                        var listObject = listLink2.Find(x => x.itemName.Contains(itemIn.col_2));
                        if (listObject != null)
                        {
                            itemIn.address_2 = listObject.address;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Thuc hien check xem item da ton tai trong listDataOut hay chua
        /// </summary>
        /// <param name="listDataOut">Danh sach khi loc da co duoc</param>
        /// <param name="item">gia tri cua item</param>
        /// <param name="typeCheck"> true: cot item dang san xuat cot 1, false: item thay the cot 2</param>
        /// <returns></returns>
        private static bool checkExistlistDataOut(List<DataOut> listDataOut, string item, bool typeCheck)
        {
            if (typeCheck == true)
            {
                foreach (var itemIn in listDataOut)
                {
                    if (itemIn.col_1 != null)
                    {
                        if (item.Contains(itemIn.col_1))
                        {
                            return true;
                        }
                    }

                }

            }
            else
            {
                foreach (var itemIn in listDataOut)
                {
                    if (itemIn.col_2 != null)
                    {
                        if (item.Contains(itemIn.col_2))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;

        }
        private static void GetComment(ref List<DataComment> listDataComment, string pathFile1)
        {
            try
            {
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                using (FileStream fs = new FileStream(pathFile1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                        for (int i = 1; i < lines.Count() - 1; i++)
                        {
                            var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab
                            listDataComment.Add(new DataComment(delimitedLine[Model.ConstSelectFile.INDEX_GET_FILE_12],
                                                       delimitedLine[Model.ConstSelectFile.INDEX_GET_COMMENT]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Lỗi xảy ra trong quá trình Get Comment: " + ex.Message));
            }
        }

        public static void MainRomveDupplicate(ref List<DataOut> listDataOut)
        {
            //Thuc hien xoa bo di du lieu thua neu vi tri == null
            foreach (var itemOut in listDataOut.ToList())
            {
                if (itemOut.address_1.Equals("null") && itemOut.address_2.Equals("null"))
                {
                    listDataOut.Remove(itemOut);
                }
            }

            //Xoa bo di gia  tri trung lap
            foreach (var itemOut in listDataOut.ToList())
            {
                var listChild = listDataOut.Where(x => x.col_1.Equals(itemOut.col_1)).ToList();
                if (itemOut.col_1.Equals(itemOut.col_2) == false)
                {
                    if (listChild.Count() >= 2)
                    {
                        listChild = listDataOut.Where(x => x.col_2.Equals(itemOut.col_2)).ToList();
                        if (listChild.Count() >= 2)
                        {
                            listDataOut.Remove(itemOut);
                        }
                    }
                }
            }
        }

        public static void GetELFileETSD(ref List<string> listE, ref List<string> listL, Input valueInput)
        {
            try
            {
                //Thuc hien lay du lieu 2 file
                GetELInfileChild(ref listE, ref listL, valueInput.file_ETSD1);
                GetELInfileChild(ref listE, ref listL, valueInput.file_ETSD2);
                RemoveDupplicate(ref listE, ref listL);
            }
            catch (Exception ex)
            {
                throw (new Exception("- Loi GetELFileETSD: " + ex.Message));
            }

        }
        private static void GetELInfileChild(ref List<string> listE, ref List<string> listL, string pathFile)
        {
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string all = sr.ReadToEnd();
                    var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                    for (int i = 1; i < lines.Count() - 1; i++)
                    {
                        var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab

                        //===> CU
                        ////Khac link lien thay the khac Y (cot S du lieu khac Y)
                        //if (delimitedLine[18].Equals("Y") == false)
                        //{

                        //    listE.Add(delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_E].Replace("\"", ""));
                        //    listL.Add(delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_L].Replace("\"", ""));
                        //}


                        //Moi
                        listE.Add(delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_E].Replace("\"", ""));
                        listL.Add(delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_L].Replace("\"", ""));

                    }
                }
            }
        }

        /// <summary>
        /// Thuc hien lay du lieu tich Y
        /// </summary>
        /// <param name="listY"></param>
        /// <param name="valueInput"></param>
        public static void GetListY(ref List<string> listY, Input valueInput)
        {
            List<string> allListY = new List<string>();
            GetListYChild(ref allListY, valueInput.file_ETSD1);//Thuc hien lay du lieu file 1 ETSD
            GetListYChild(ref allListY, valueInput.file_ETSD2);//Thuc hien lay du lieu file 2 ETSD

            listY = allListY.Distinct().ToList();

        }
        private static void GetListYChild(ref List<string> listY, string pathFile)
        {
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string all = sr.ReadToEnd();
                    var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                    for (int i = 1; i < lines.Count() - 1; i++)
                    {
                        var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab

                        //Lay du lieu tich Y
                        if (delimitedLine[18].Contains("Y") == true)
                        {
                            listY.Add(delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_E].Replace("\"", ""));
                            listY.Add(delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_L].Replace("\"", ""));
                        }

                    }
                }
            }
        }

        public static void AddCommentY(ref List<DataOut> listDataOut, List<string> listCheckY)
        {
            if (listCheckY.Count == 0)
            {
                return;
            }

            foreach (var item in listDataOut)
            {
                if (item.col_1 != null)
                {
                    if (listCheckY.Contains(item.col_1))
                    {
                        item.comment_1 = "(@Y)" + item.comment_1;
                    }
                }//Thuc hien kiem tra trong tich Y cot 1
                if (item.col_2 != null)
                {
                    if (listCheckY.Contains(item.col_2))
                    {
                        item.comment_2 = "(@Y)" + item.comment_2;
                    }
                }//Thuc hien kiem tra trong tich Y cot 2
            }
        }

    }

}
