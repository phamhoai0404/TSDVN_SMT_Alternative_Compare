using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Alternative_Compare.Model
{
    public class ResultSelectFile
    {
        public const string NOT_SELECT_FILE = null;
        public const string TO_CATCH = "-1";
    }
    public class ConstSelectFile
    {
        public const string TYPE_FILE = "Excel file: |*.csv";
        public const char TYPE_SEPARATOR = ',';
        public const char TYPE_LINE_MARK = '\n';
        public const int INDEX_GET_FILE_12 = 10;
        public const int INDEX_GET_ETSD_E = 4;
        public const int INDEX_GET_ETSD_L = 11;
    }
    public class TypeClick
    {
        public const string FILE_1 = "FILE_1";
        public const string FILE_2= "FILE_2";
        public const string FILE_DOWNLOAD_ETSD = "FILE_DOWNLOAD_ETSD";
        public const string FILE_LINK_DATA = "FILE_LINK_DATA";
    }
    public class ResultCheckInput
    {
        //Loi bo trong dong du lieu nao do
        public const string ERROR_EMPTY = "ERROR_EMPTY";

        //Trung file trong du lieu nhap vao
        public const string ERROR_DUPLICATE_FILE= "ERROR_DUPLICATE_FILE";

        //Loi khong ton tai file 1
        public const string ERROR_EXIST_FILE_1 = "ERROR_EXIST_FILE_1";

        //Loi khong ton tai file 2
        public const string ERROR_EXIST_FILE_2 = "ERROR_EXIST_FILE_2";

        //Loi khong ton tai file ETSD
        public const string ERROR_EXIST_FILE_ETSD = "ERROR_EXIST_FILE_ETSD";

        //Loi khong ton tai file link data
        public const string ERROR_EXIST_FILE_LINK_DATA = "ERROR_EXIST_FILE_LINK_DATA";

        //OK
        public const string OK = "OK";
    }


    public class ResultCompareTwoFile
    {
        public const string OK = "OK";
        public const string INFOR_EMPTY = "INFOR_EMPTY";
    }

    public class ResultCompareETSD
    {
        public const string OK = "OK";
        public const string INFOR_EMPTY = "INFOR_EMPTY";
    }


}
