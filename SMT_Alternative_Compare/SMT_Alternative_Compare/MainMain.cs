using SMT_Alternative_Compare.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SMT_Alternative_Compare
{
    public partial class MainMain : Form
    {
        private Input valueInput = new Input();
        public MainMain()
        {
            InitializeComponent();
        }
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            this.txtFile2.Clear();
            this.txtFileETSD.Clear();
            this.txtFileLinkData.Clear();
            this.txtFile1.Clear();
            this.txtFile1.Focus();
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            try
            {
                ////Thuc hien check du lieu dau vao
                //this.actionButton(false);
                //this.updateLable("Check dữ liệu đầu vào");
                ////Neu du lieu dau vao khong Ok thi dung lai
                //if (ValidateInput() == false)
                //{
                //    return;
                //}


                //test thôi
                this.GetInput();

                //Thuc hien lay du lieu 2 file ban dau
                List<string> listFile1 = Function.MyFunction.GetDataItemFile(this.valueInput.file_1, Model.ConstSelectFile.INDEX_GET_FILE_12);
                List<string> listFile2 = Function.MyFunction.GetDataItemFile(this.valueInput.file_2, Model.ConstSelectFile.INDEX_GET_FILE_12);

                //Thuc hien so sanh loai bo di du lieu giong nhau
                string resultCompare = Function.MyFunction.CompareTwoFile(ref listFile1, ref listFile2);
                switch (resultCompare)
                {
                    case Model.ResultCompareTwoFile.OK:
                        break;
                    case Model.ResultCompareTwoFile.INFOR_EMPTY:
                        MessageBox.Show("Hai file 1 và 2 không có linh kiện đặc biệt!", "Info Compare Two File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    default:
                        MessageBox.Show("Đã có lỗi xảy ra trong quá trình so sánh hai file 1 và 2: " + resultCompare, "Error Compare Two File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                //Thuc hien lay du lieu file ETSD
                List<string> listFile_ETSD_E = Function.MyFunction.GetDataItemFile(this.valueInput.file_ETSD, Model.ConstSelectFile.INDEX_GET_ETSD_E);
                List<string> listFile_ETSD_L= Function.MyFunction.GetDataItemFile(this.valueInput.file_ETSD, Model.ConstSelectFile.INDEX_GET_ETSD_L);
                if(listFile_ETSD_E.Count != listFile_ETSD_L.Count)
                {
                    MessageBox.Show("Có lỗi xảy ra trong quá trình lấy dữ liệu của file ETSD vì số lượng linh kiện chính và linh kiện thay thế không bằng nhau", "Error Get File ETSD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                List<DataOut> listDataOut = new List<DataOut>();
                resultCompare = Function.MyFunction.ActionCompareETSD(listFile1, listFile2, listFile_ETSD_E, listFile_ETSD_L, ref listDataOut);
                switch (resultCompare)
                {
                    case Model.ResultCompareETSD.OK:
                        break;
                    case Model.ResultCompareETSD.INFOR_EMPTY:
                        MessageBox.Show("Không cặp linh kiện thay thế nào thỏa mãn!", "Info Compare ETSD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    default:
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình so sánh 2 file với file ETSD: " + resultCompare, "Error Compare ETSD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }
                this.dgvResult.DataSource = listDataOut;

            }
            finally
            {
                this.actionButton(true);
            }


        }

        #region Action Select file
        private void btnSelectFile1_Click(object sender, EventArgs e)
        {
            this.ClickSelectFile(Model.TypeClick.FILE_1);
        }

        private void btnSelectFile2_Click(object sender, EventArgs e)
        {
            this.ClickSelectFile(Model.TypeClick.FILE_2);
        }

        private void btnSelectFileETSD_Click(object sender, EventArgs e)
        {
            this.ClickSelectFile(Model.TypeClick.FILE_DOWNLOAD_ETSD);
        }

        private void btnSelectFileLinkData_Click(object sender, EventArgs e)
        {
            this.ClickSelectFile(Model.TypeClick.FILE_LINK_DATA);
        }

        /// <summary>
        /// Thuc hien hanh dong khi click vao file
        /// </summary>
        /// <param name="typeClick">Loai hanh dong khi click vao</param>
        /// CreatedBy: HoaiPT(02/02/2023)
        private void ClickSelectFile(string typeClick)
        {
            string tempResult = Function.MyFunction.SelectFile();
            switch (tempResult)
            {
                case Model.ResultSelectFile.NOT_SELECT_FILE:
                    return;
                case Model.ResultSelectFile.TO_CATCH:
                    MessageBox.Show("Đã có lỗi xảy ra trong quá trình select file!", "Error Select File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                default:
                    switch (typeClick)
                    {
                        case Model.TypeClick.FILE_1:
                            this.txtFile1.Text = tempResult;
                            break;
                        case Model.TypeClick.FILE_2:
                            this.txtFile2.Text = tempResult;
                            break;
                        case Model.TypeClick.FILE_DOWNLOAD_ETSD:
                            this.txtFileETSD.Text = tempResult;
                            break;
                        case Model.TypeClick.FILE_LINK_DATA:
                            this.txtFileLinkData.Text = tempResult;
                            break;
                    }
                    break;
            }
        }

        #endregion

        #region Check Du lieu dau vao
        /// <summary>
        /// Hanh dong lay du lieu dau vao
        /// </summary>
        private void GetInput()
        {
            this.valueInput.file_1 = this.txtFile1.Text;
            this.valueInput.file_2 = this.txtFile2.Text;
            this.valueInput.file_ETSD = this.txtFileETSD.Text;
            this.valueInput.fileLinkData = this.txtFileLinkData.Text;
        }

        private bool ValidateInput()
        {
            this.GetInput();
            string resultValidate = Function.MyFunction.CheckInput(this.valueInput);
            switch (resultValidate)
            {
                case Model.ResultCheckInput.OK:
                    return true;
                case Model.ResultCheckInput.ERROR_EMPTY:
                    MessageBox.Show("Không được để trống các ô textbox !!!", "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_DUPLICATE_FILE:
                    MessageBox.Show("Dữ liệu các file có sự trùng lặp hãy kiểm tra lại!", "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_1:
                    MessageBox.Show("File 1 không tồn tại:" + valueInput.file_1, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_2:
                    MessageBox.Show("File 2 không tồn tại:" + valueInput.file_2, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_ETSD:
                    MessageBox.Show("File ETSD không tồn tại:" + valueInput.file_ETSD, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_LINK_DATA:
                    MessageBox.Show("File Link data không tồn tại:" + valueInput.fileLinkData, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                default:
                    MessageBox.Show("Đã có lỗi xảy ra trong quá trình check dữ liệu đầu vào CheckInput-Catch:" + resultValidate, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
            }
        }
        #endregion

        private void MainMain_Load(object sender, EventArgs e)
        {
            this.actionButton(true);

            this.txtFile1.Text = @"C:\Users\Sys009\Desktop\Hoai_Daotao\vs\RunShowPowerPoint\data_test\smt_\temp\XRHP06609.csv";
            this.txtFile2.Text = @"C:\Users\Sys009\Desktop\Hoai_Daotao\vs\RunShowPowerPoint\data_test\smt_\temp\XRHP06610.csv";
            this.txtFileETSD.Text = @"C:\Users\Sys009\Desktop\Hoai_Daotao\vs\RunShowPowerPoint\data_test\smt_\temp\LKTT.csv";
            //this.txtFile1.Text = @"\\192.168.3.6\public\96. Share Data\99. Other\13. IT\HOAI\SMT-Compare_A\XRHP06609.csv";
            //this.txtFile2.Text = @"P:\96. Share Data\99. Other\13. IT\HOAI\SMT-Compare_A\XRHP06610.csv";
            //this.txtFileETSD.Text = @"P:\96. Share Data\99. Other\13. IT\HOAI\SMT-Compare_A\LKTT.csv";
        }
        #region Action Style
        /// <summary>
        /// Thuc hien set nut hanh dong trang thai
        /// </summary>
        /// <param name="action"></param>
        /// CreatedBy: HoaiPT(?/?/2022)
        private void actionButton(bool action)
        {
            if (action == true)
            {
                this.picExecute.Visible = false;
                this.picDone.Visible = true;
                this.pnlMain.Enabled = true;
                this.pnlMain2.Enabled = true;
                this.updateLable("Sẵn sàng thực hiện");
            }
            else
            {
                this.pnlMain.Enabled = false;
                this.pnlMain2.Enabled = false;
                this.picDone.Visible = false;
                this.picExecute.Visible = true;
            }

            this.pnlMain.Enabled = action;
            this.picExecute.Update();
            this.picDone.Update();
        }
        /// <summary>
        /// Thuc hien update label 
        /// </summary>
        /// <param name="nameText">Ten label muon cap nhat</param>
        /// CreatedBy: HoaiPT(?/?/2022)
        private void updateLable(string nameText)
        {
            this.lblDisplay.Text = nameText;
            this.lblDisplay.Update();
        }

        #endregion
    }
}
