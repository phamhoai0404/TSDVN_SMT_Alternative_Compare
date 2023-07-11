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

        

        public DataOut(string value1, string value2, string tempMain = null)
        {
            this.col_1 = value1;
            this.col_2 = value2;
            this.tempMain = tempMain;
        }
        public void addAdress(string address1, string address2)
        {
            this.address_1 = address_1;
            this.address_2 = address_2;
        }
        public DataOut(string item, string comment, int type)
        {
            if(type == 1)
            {
                this.col_1 = item;
                this.comment_1 = comment;
                this.address_1 = "OTHER";
            }
            else
            {
                this.col_2 = item;
                this.comment_2 = comment;
                this.address_2 = "OTHER";
            }
            
        }
        


    }
    public class DataEL
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public DataEL(string add1, string add2)
        {
            this.address1 = add1;
            this.address2 = add2;
        }
    }
    public class DataLink
    {
        public string address;
        public string itemName;
        public DataLink(string address, string itemName)
        {
            this.address = address;
            this.itemName = itemName;
        }
    }
    public class DataComment
    {
        public string item { get; set; }
        public string comment { get; set; }

        public DataComment(string item, string comment)
        {
            this.item = item;
            this.comment = comment;
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
