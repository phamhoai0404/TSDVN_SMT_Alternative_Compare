using SMT_Alternative_Compare.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMT_Alternative_Compare.Function
{
    public class MyFunction
    {
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
        public static string CheckInput(Input valueInput)
        {
            try
            {


                //Check dau vao khong duoc de trong
                if (string.IsNullOrWhiteSpace(valueInput.file_1) ||
                   string.IsNullOrWhiteSpace(valueInput.file_2) ||
                   string.IsNullOrWhiteSpace(valueInput.file_ETSD) ||
                   string.IsNullOrWhiteSpace(valueInput.fileLinkData))
                {
                    return Model.ResultCheckInput.ERROR_EMPTY;
                }

                //Thuc hien bo di cac ki tu thua neu co
                valueInput.file_1 = valueInput.file_1.Trim();
                valueInput.file_2 = valueInput.file_2.Trim();
                valueInput.file_ETSD = valueInput.file_ETSD.Trim();
                valueInput.fileLinkData = valueInput.fileLinkData.Trim();


                //Kiem tra 2 file dau vao co trung nhau hay khong
                if (valueInput.file_1.Equals(valueInput.file_2) ||
                    valueInput.file_1.Equals(valueInput.file_ETSD) ||
                    valueInput.file_1.Equals(valueInput.fileLinkData) ||
                    valueInput.file_2.Equals(valueInput.file_ETSD) ||
                    valueInput.file_2.Equals(valueInput.fileLinkData) ||
                    valueInput.file_ETSD.Equals(valueInput.fileLinkData))
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
                if (!File.Exists(valueInput.file_ETSD))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_ETSD;
                }
                if (!File.Exists(valueInput.file_1))
                {
                    return Model.ResultCheckInput.ERROR_EXIST_FILE_LINK_DATA;
                }




                return Model.ResultCheckInput.OK;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Thuc hien lay du lieu cac item cua file
        /// </summary>
        /// <param name="pathFile">Dia chi file</param>
        /// <param name="indexColumn">Dia chi cot du lieu muon lay</param>
        /// <param name="hasHeader">Ton  tai tieu de trong du lieu hay khong</param>
        /// <returns></returns>
        /// CreatedBy: HoaiPT(02/02/2023)
        public static List<string> GetDataItemFile(string pathFile, int indexColumn, bool hasHeader = true)
        {
            //try
            //{
                //Bien lay du lieu
                int indexStart = 0;
                if (hasHeader)
                {
                    indexStart = 1;//Neu hasHeader = true => Co tieu de se bo di tieu de
                }

                List<string> listFile1 = new List<string>();
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                //String[] Fields = CSVParser.Split(Test);

                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                        for (int i = indexStart; i < lines.Count() - 1; i++)
                        {
                            var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab
                            listFile1.Add(delimitedLine[indexColumn]);
                        }
                    }
                }
                return listFile1;
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }

        /// <summary>
        /// Thuc hien so sanh du lieu cua 2 list va xoa bo di du lieu giong nhau
        /// </summary>
        /// <param name="listFile1">Du lieu file 1</param>
        /// <param name="listFile2">Du lieu file 2</param>
        /// <returns></returns>
        public static string CompareTwoFile(ref List<string> listFile1, ref List<string> listFile2)
        {
            try
            {
                foreach (var item in listFile1.ToList())
                {
                    bool checkExistItem = listFile2.Contains(item);
                    if (checkExistItem)
                    {
                        listFile1.Remove(item);
                        listFile2.Remove(item);
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

        public static string ActionCompareETSD(List<string> list1, List<string> list2, List<string> listETSD_E, List<string> listETSD_L, ref List<DataOut> listResult)
        {
            try
            {
                foreach (var item in list1)
                {
                    List<DataEL> tempDataEL = new List<DataEL>();
                    for (int i = 0; i < listETSD_E.Count; i++)
                    {
                        if (item.Equals(listETSD_E[i]))
                        {
                            tempDataEL.Add(new DataEL(listETSD_E[i], listETSD_L[i]));
                        }
                    }

                    if (tempDataEL.Count == 0)
                    {

                        for (int i = 0; i < listETSD_L.Count; i++)
                        {
                            if (item.Equals(listETSD_L[i]))
                            {
                                tempDataEL.Add(new DataEL(listETSD_L[i], listETSD_E[i]));
                            }
                        }
                        if (tempDataEL.Count == 0)
                        {
                            continue;
                        }
                    }

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
                
                if(listResult.Count == 0)
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
    }

}
