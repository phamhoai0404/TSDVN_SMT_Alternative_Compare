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
    public class MyFunction2
    {
        public static string CheckWOFirst(string fileWOFirst, ref Input valueInput)
        {
            try
            {
                //Check khong duoc de trong
                if (string.IsNullOrWhiteSpace(fileWOFirst))
                {
                    return MdlCommon.ERROR_001;
                }
                fileWOFirst = fileWOFirst.Trim();//Loai bo di ki tu thua
                if (!File.Exists(fileWOFirst))
                {
                    return MdlCommon.ERROR_002;
                }//Check su ton tai cua file
                //Check dinh dang file .csv
                if (!Function.MyFunction.CheckFileCSV(fileWOFirst))
                {
                    return MdlCommon.ERROR_003;//Khong phai dinh dang file .csv
                }

                string pathFolder = Path.GetDirectoryName(fileWOFirst);//Thuc hien lay ten
                List<string> listFileCSV = Directory.GetFiles(pathFolder, "*.csv").ToList();
                if(!(listFileCSV.Count==4 || listFileCSV.Count == 6))//Truong hop = 5 file khi trung model
                {
                    return MdlCommon.ERROR_004;
                }




                return MdlCommon.OK;
            }
            catch (Exception ex)
            {
                return string.Format(MdlCommon.ERROR_015_CATCH, "CheckWOFirst", ex.Message);
            }
        }
        public static string FilePicklist(string pathFile)
        {
            try
            {
                
                string temp = "";
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);

                        for (int i = 0; i < lines.Count() - 1; i++)
                        {
                            var delimitedLine = CSVParser.Split(lines[i]); //set ur separator, in this case tab
                            temp = delimitedLine[indexColumn].Replace("\"", "");
                            listFile1.Add(temp);
                        }
                    }
                }
               
            }
            catch (Exception)
            {
                return string.Format(MdlCommon.ERROR_015_CATCH, "CheckWOFirst", ex.Message);
            }
        }
    }
}
