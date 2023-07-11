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
    class ActionFeeder
    {
        /// <summary>
        /// Thuc hien lay du lieu file CSV
        /// </summary>
        /// <param name="pathFile1"></param>
        /// <param name="listFeeder"></param>
        public static void GetDataCSV(string pathFile1, ref List<Feeder> listFeeder)
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
                            if (delimitedLine?.Count() >= 2)
                            {
                                listFeeder.Add(new Feeder(delimitedLine[Model.ConstDataLink.INDEX_GET_ITEM]?.Replace("\"", "").Trim().ToUpper(),
                                                      delimitedLine[Model.ConstDataLink.INDEX_GET_ADDRESS]?.Replace("\"", "").Trim()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ActionFeeder: GetDataLinkAddress - " + ex.Message);
            }
        }

        /// <summary>
        /// Thuc hien lay gan dia chi 
        /// </summary>
        /// <param name="listDataOut"></param>
        /// <param name="listFeeder_1"></param>
        /// <param name="listFeeder_2"></param>
        /// <param name="listLKTT"></param>
        public static void AddAddress(ref List<DataOut> listDataOut, List<Feeder> listFeeder_1, List<Feeder> listFeeder_2, List<LKTT_ETSD> listLKTT)
        {
            foreach (var itemCurrent in listDataOut)
            {
                //Truong hop duyet roi thi thoi
                if(itemCurrent.tempMain != null)
                {
                    if(itemCurrent.tempMain == MdlCommn.EXIST_ALL_TEMPMAIN)
                        continue;
                }
                
                bool checkAddress_1 = false, checkAddress_2 = false;

                if (string.IsNullOrEmpty(itemCurrent.col_1) == false)
                {
                    var objectOne = listFeeder_1.FirstOrDefault(p => p.feederItem == itemCurrent.col_1);
                    if (objectOne != null)
                    {
                        itemCurrent.address_1 = objectOne.feederAddress;
                    }
                    else
                    {
                        var objectRef = listFeeder_1.FirstOrDefault(x => x.feederItem == itemCurrent.col_2);//Thuc hien xet item 2 trong feeder cua no
                        if (objectRef != null)
                        {
                            itemCurrent.address_1 = objectRef.feederAddress;
                            checkAddress_1 = true;
                        }
                        else
                        {
                            itemCurrent.address_1 = MdlCommn.NULL_ADDRESS;//Sau khi duyet no va duyet con ben canh no van khong co du lieu thi ghi la NO
                        }
                    }
                }
                else
                {
                    checkAddress_1 = true;//Truong hop khong co du lieu
                }

                if (string.IsNullOrEmpty(itemCurrent.col_2) == false)
                {
                    var objectTwo = listFeeder_2.FirstOrDefault(p => p.feederItem == itemCurrent.col_2);
                    if (objectTwo != null)
                    {
                        itemCurrent.address_2 = objectTwo.feederAddress;
                    }
                    else
                    {
                        var objectRef = listFeeder_2.FirstOrDefault(x => x.feederItem == itemCurrent.col_1);
                        if (objectRef != null)
                        {
                            itemCurrent.address_2 = objectRef.feederAddress;
                            checkAddress_2 = true;
                        }
                        else
                        {
                            itemCurrent.address_2 = MdlCommn.NULL_ADDRESS;
                        }
                    }
                }
                else
                {
                    checkAddress_2 = true;
                }

                if ((string.IsNullOrWhiteSpace(itemCurrent.tempMain) == false) &&
                    (checkAddress_1 == false || checkAddress_2 == false))
                {
                    //Nhom theo group
                    var groupLKTT = listLKTT.Where(p => p.dataE == itemCurrent.tempMain).ToList();
                    if(checkAddress_1 == false)
                    {
                        foreach (var itemG in groupLKTT)
                        {
                            var checkObject = listFeeder_1.FirstOrDefault(p => p.feederItem == itemG.dataE || p.feederItem == itemG.dataL);
                            if(checkObject != null)
                            {
                                itemCurrent.address_1 = checkObject.feederAddress;
                                break;
                            }
                        }
                    }
                    if(checkAddress_2 == false)
                    {
                        foreach (var itemG in groupLKTT)
                        {
                            var checkObject = listFeeder_2.FirstOrDefault(p => p.feederItem == itemG.dataE || p.feederItem == itemG.dataL);
                            if (checkObject != null)
                            {
                                itemCurrent.address_2 = checkObject.feederAddress;
                                break;
                            }
                        }
                    }
                }
               
            }
        }
    }
}
