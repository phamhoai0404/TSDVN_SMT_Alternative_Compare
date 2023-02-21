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
        public static string CheckWOFirst(string fileWOFirst, ref Input valueOut)
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
                if (!(listFileCSV.Count == 4 || listFileCSV.Count == 6))//Truong hop = 4 file khi trung model
                {
                    return MdlCommon.ERROR_004;
                }
                listFileCSV.Remove(fileWOFirst);
                valueOut.file_1 = fileWOFirst;

                GetFileWO wo1 = new GetFileWO();
                string result = FilePicklistWO1(fileWOFirst, ref wo1);
                if (!result.Equals(MdlCommon.OK))//Neu co loi xay ra thi dung lai
                {
                    return result;
                }
                if (wo1.typeFileWO == false)//Khong thuoc loai file WO
                {
                    return MdlCommon.ERROR_005 + fileWOFirst;
                }

                //Check file chua ten model
                foreach (var item in listFileCSV)
                {
                    if (item.Contains(wo1.modelName))
                    {
                        valueOut.fileLinkData1 = item;
                        break;
                    }
                }
                if (valueOut.fileLinkData1 == null)
                {
                    return string.Format(MdlCommon.ERROR_006, pathFolder, wo1.modelName);
                }
                listFileCSV.Remove(valueOut.fileLinkData1);

                GetFileWO wo2 = new GetFileWO();
                wo2.typeFileWO = false;
                List<GetFile> listETSD = new List<GetFile>();
                foreach (var item in listFileCSV.ToList())
                {
                    GetFile valueFile = new GetFile();
                    result = FileGet(item, ref valueFile);
                    if (!result.Equals(MdlCommon.OK))
                    {
                        return result;
                    }
                    switch (valueFile.typeFile)
                    {
                        case "ETSD":
                            if (valueFile.modelName.Equals(wo1.modelName))
                            {
                                valueOut.file_ETSD1 = item;
                                listFileCSV.Remove(item);//Xoa di
                            }
                            else
                            {
                                listETSD.Add(valueFile);
                            }

                            break;
                        case "PICKLIST":
                            valueOut.file_2 = item;
                            wo2.typeFileWO = true;
                            wo2.nameWO = valueFile.nameWO;
                            wo2.modelName = valueFile.modelName;

                            listFileCSV.Remove(item);
                            break;
                    }
                }
                //Truong hop loi ETSD
                if (valueOut.file_ETSD1 == null)
                {
                    return string.Format(MdlCommon.ERROR_007, pathFolder, wo1.modelName);
                }
                if (listETSD.Count > 1)
                {
                    return string.Format(MdlCommon.ERROR_009, pathFolder);
                }

                if (valueOut.file_2 == null)
                {
                    return string.Format(MdlCommon.ERROR_008, pathFolder);
                }
                foreach (var item in listFileCSV)
                {
                    if (item.Contains(wo2.modelName))
                    {
                        valueOut.fileLinkData2 = item;
                        break;
                    }
                }
                if (valueOut.fileLinkData2 == null)
                {
                    if (wo1.modelName.Equals(wo2.modelName))
                    {
                        valueOut.fileLinkData2 = valueOut.fileLinkData1;
                    }
                    else
                    {
                        return string.Format(MdlCommon.ERROR_006, pathFolder, wo2.modelName);
                    }
                }

                //Kiem tra neu ten model cua ETSD = WO2 thi de lai
                if (listETSD[0].modelName.Equals(wo2.modelName))
                {
                    valueOut.file_ETSD2 = listETSD[0].pathFile;
                }
                else
                {
                    if (wo1.modelName.Equals(wo2.modelName))
                    {
                        valueOut.file_ETSD2 = valueOut.file_ETSD1;
                    }
                    else
                    {
                        return string.Format(MdlCommon.ERROR_007, pathFolder, wo2.modelName);
                    }
                }


               // string k = "9";


                return MdlCommon.OK;
            }
            catch (Exception ex)
            {
                return string.Format(MdlCommon.ERROR_015_CATCH, "CheckWOFirst", ex.Message);
            }
        }
        public static string FilePicklistWO1(string pathFile, ref GetFileWO valueOut)
        {
            try
            {
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);
                        valueOut.typeFileWO = false;
                        var delimitedLine = CSVParser.Split(lines[0]);
                        if (delimitedLine[1].Contains("WO_NUM"))
                        {
                            valueOut.typeFileWO = true;
                            delimitedLine = CSVParser.Split(lines[1]);
                            valueOut.nameWO = delimitedLine[ConstSelectFile.INDEX_GET_NAME_WO].Replace("\"", "");
                            valueOut.modelName = delimitedLine[ConstSelectFile.INDEX_GET_NAME_MODEL].Replace("\"", "");
                        }
                    }
                }
                return MdlCommon.OK;

            }
            catch (Exception ex)
            {
                return string.Format(MdlCommon.ERROR_015_CATCH, "FilePicklistWO1", ex.Message);
            }
        }

        public static string FileGet(string pathFile, ref GetFile valueOut)
        {
            try
            {
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split(Model.ConstSelectFile.TYPE_LINE_MARK);
                       
                        valueOut.pathFile = pathFile;

                        var delimitedLine = CSVParser.Split(lines[0]);
                        if (delimitedLine[1].Contains("MODEL"))//Thuoc loai ETSD
                        {
                            valueOut.typeFile = "ETSD";
                            delimitedLine = CSVParser.Split(lines[1]);
                            valueOut.modelName = delimitedLine[1].Replace("\"", "");
                            
                        }
                        else
                        {
                            if (delimitedLine[1].Contains("WO_NUM")) //Thuoc loai WO
                            {
                                valueOut.typeFile = "PICKLIST";
                                delimitedLine = CSVParser.Split(lines[1]);
                                valueOut.nameWO = delimitedLine[ConstSelectFile.INDEX_GET_NAME_WO].Replace("\"", "");
                                valueOut.modelName = delimitedLine[ConstSelectFile.INDEX_GET_NAME_MODEL].Replace("\"", "");
                            }
                            else
                            {
                                if (delimitedLine[1].Contains("PartNumber"))
                                {
                                    valueOut.typeFile = "FEEDER";
                                }
                                else
                                {
                                    valueOut.typeFile = "NULL";
                                }
                            }

                        }
                    }
                }
                return MdlCommon.OK;

            }
            catch (Exception ex)
            {
                return string.Format(MdlCommon.ERROR_015_CATCH, "FileGet", pathFile + " - " + ex.Message);
            }
        }
    }
}
