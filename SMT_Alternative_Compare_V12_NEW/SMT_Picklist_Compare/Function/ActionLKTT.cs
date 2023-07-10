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
    public class ActionLKTT
    {
        /// <summary>
        /// Thuc hien lay du lieu file CSV
        /// </summary>
        /// <param name="pathFile"></param>
        /// <param name="listData"></param>
        public static void GetDataCSV(string pathFile, ref List<LKTT_ETSD> listData)
        {
            try
            {
                string tempE = "", tempL = "", tempY = "";
                bool checkY;
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
                            if (delimitedLine.Length < 18)
                            {
                                continue;
                            }

                            tempE = delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_E]?.Replace("\"", "").Trim().ToUpper();
                            tempL = delimitedLine[Model.ConstSelectFile.INDEX_GET_ETSD_L]?.Replace("\"", "").Trim().ToUpper();
                            tempY = delimitedLine[18]?.ToUpper();

                            if (tempY.Contains("Y"))
                            {
                                checkY = true;
                            }
                            else
                            {
                                checkY = false;
                            }

                            var checkExist = listData.FirstOrDefault(x => x.dataE == tempE && x.dataL == tempL);
                            if (checkExist == null)
                            {
                                listData.Add(new LKTT_ETSD(tempE, tempL, checkY));//Thuc hien add du lieu
                            }
                            else
                            {
                                //Thuc hien xet truong hop con dang co trong list = false va con moi =  true thi can phai cap nhat lai
                                if (checkY && checkExist.checkY == false)
                                {
                                    foreach (var itemIn in listData)
                                    {
                                        if (itemIn.dataE == tempE && itemIn.dataL == tempL)
                                        {
                                            itemIn.checkY = true;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ActionLKTT: GetDataCSV - PathFile - {pathFile}: {ex.Message}");
            }
        }

        /// <summary>
        /// Function thuc hien xoa bo cac gia tri trung lap
        /// </summary>
        /// <param name="listData"></param>
        public static void RemoveDupplicate(ref List<LKTT_ETSD> listData)
        {
            for (int i = 0; i < listData.Count; i++)
            {
                for (int j = i + 1; j < listData.Count; j++)
                {
                    if (listData[i].dataE == listData[j].dataE && listData[i].dataL == listData[j].dataL)
                    {
                        listData.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

       
    }
}
