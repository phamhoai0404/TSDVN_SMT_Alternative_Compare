using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Model
{
    public class DataOut
    {
        public string col_1 { get; set; }
        public string address_1 { get; set; }
        public string comment_1 { get; set; }
        public string col_2 { get; set; }
        public string address_2 { get; set; }
        public string comment_2 { get; set; }
        public string tempMain { get; set; }

        public DataOut()
        {

        }
        

        public DataOut(string value1, string value2, bool existAll)
        {
            this.col_1 = value1;
            this.col_2 = value2;
            this.tempMain = null;
            if (existAll)
            {
                this.comment_1 = MdlCommn.EXIST_ALL_EL;
                this.comment_2 = MdlCommn.EXIST_ALL_EL;
            }
        }
        public DataOut(string value1, string value2, string tempMain, bool existAll)
        {
            this.col_1 = value1;
            this.col_2 = value2;
            this.tempMain = tempMain;
            if (existAll)
            {
                this.comment_1 = MdlCommn.EXIST_ALL_EL;
                this.comment_2 = MdlCommn.EXIST_ALL_EL;
            }
        }

        public void addAdress(string address1, string address2)
        {
            this.address_1 = address_1;
            this.address_2 = address_2;
        }
        
        public DataOut(DataOut s)
        {
            this.col_1 = s.col_1;
            this.col_2 = s.col_2;
            this.comment_1 = s.comment_1;
            this.comment_2 = s.comment_2;
            this.address_1 = s.address_1;
            this.address_2 = s.address_2;
        }
       
        


    }
   
    public class GetInfo
    {
        public string wo1 { get; set; }
        public string wo2 { get; set; }
        public string model1 { get; set; }
        public string model2 { get; set; }

    }
    public class GetFileWO
    {
        public string modelName { get; set; }
        public string nameWO { get; set; }
        public bool typeFileWO { get; set; }
    }
    public class GetFile
    {
        public string typeFile { get; set; }
        public string modelName { get; set; }
        public string nameWO { get; set; }
        public string pathFile { get; set; }

        public GetFile()
        {

        }
        public GetFile(GetFile s)
        {
            this.typeFile = s.typeFile;
            this.modelName = s.modelName;
            this.nameWO = s.nameWO;
            this.pathFile = s.pathFile;
        }
    }
}
