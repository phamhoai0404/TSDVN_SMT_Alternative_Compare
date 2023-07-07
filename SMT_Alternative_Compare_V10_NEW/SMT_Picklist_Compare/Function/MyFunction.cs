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

 

    }

}
