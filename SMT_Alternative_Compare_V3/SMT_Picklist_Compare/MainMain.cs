using SMT_Picklist_Compare.Model;
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

namespace SMT_Picklist_Compare
{
    public partial class MainMain : Form
    {
        private Input valueInput = new Input();
        public MainMain()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Bien thuc hien de luu tru danh sach cac cap phu hop
        /// </summary>
        /// UpdateBy: HoaiPT(06/02/2023)
        List<DataOut> listDataOut = new List<DataOut>();
        private GetInfo getInfo = new GetInfo();

        /// <summary>
        /// Hanh dong khi click vao nut ClearDataAll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// UpdateBy: HoaiPT(06/02/2023)
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            this.txtFileWOFirst.Clear();

            //Thuc hien clear du lieu ban dau
            this.ClearDataFirst();

            this.ClearDataResult();
            this.txtFileWOFirst.Focus();
        }
        /// <summary>
        /// Thuc hien xoa bo du lieu cu da tinh toan
        /// </summary>
        /// CreatedBy: HoaiPT(06/02/2023)
        private void ClearDataResult()
        {
            this.dgvResult.DataSource = null;//Reset ve null
            this.lblWO1.Text = "No 1";//set null
            this.lblWO2.Text = "No 2";//set null
        }
        private void ClearDataFirst()
        {
            this.txtFile2.Clear();
            this.txtFileETSD1.Clear();
            this.txtFileLinkData1.Clear();
            this.txtFile1.Clear();
            this.txtFile1.Focus();
            this.txtFileLinkData2.Clear();
            this.txtSearch.Clear();
            this.txtFileETSD2.Clear();
        }
        private void btnAction_Click(object sender, EventArgs e)
        {
            try
            {
                //Thuc hien check du lieu dau vao
                this.actionButton(false);
                this.updateLable("Check dữ liệu đầu vào");
                //Neu du lieu dau vao khong Ok thi dung lai
                if (ValidateInput() == false)
                {
                    return;
                }
                this.updateLable("Thực hiện xóa dữ liệu cũ");
                this.listDataOut.Clear();
                this.ClearDataResult();


                this.updateLable("Lấy dữ liệu 2 file Picklist");
                //Thuc hien lay du lieu 2 file ban dau
                string nameWO1 = Model.ConstSelectFile.TYPE_GET_NAME_WO;//Bien luu tru ten WO1
                string nameWO2 = Model.ConstSelectFile.TYPE_GET_NAME_WO;//Bien luu tru ten WO2
                List<string> listFile1 = Function.MyFunction.GetDataItemFile(this.valueInput.file_1, Model.ConstSelectFile.INDEX_GET_FILE_12, ref nameWO1);
                List<string> listFile2 = Function.MyFunction.GetDataItemFile(this.valueInput.file_2, Model.ConstSelectFile.INDEX_GET_FILE_12, ref nameWO2);
                this.lblWO1.Text = nameWO1;
                this.lblWO2.Text = nameWO2;

                listFile1 = listFile1.Distinct().ToList();//Xoa bo di gia tri trung lap
                listFile2 = listFile2.Distinct().ToList();//Xoa bo di gia tri trung lap

                List<string> listFile_ETSD_E = new List<string>();
                List<string> listFile_ETSD_L = new List<string>();

                Function.MyFunction.GetELFileETSD(ref listFile_ETSD_E, ref listFile_ETSD_L, this.valueInput);

                this.updateLable("Thực hiện xóa cặp item giống nhau trong 2 file");// giá trị giống nhau nhưng không tồn tại 
                string resultCompare = Function.MyFunction.CompareTwoFile(ref listFile1, ref listFile2, listFile_ETSD_E, listFile_ETSD_L);
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

                this.updateLable("Thực hiện so sánh 2 file với file ETSD");
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

                this.updateLable("Thực hiện thêm địa chỉ cho từng item");
                string resultAddAddress = Function.MyFunction.Add_Address(ref listDataOut, this.valueInput.fileLinkData1, this.valueInput.fileLinkData2);
                switch (resultAddAddress)
                {
                    case Model.ResultAddAddress.OK:
                        break;
                    case Model.ResultAddAddress.ERROR_GET_LINK_1:
                        MessageBox.Show("Lỗi xảy ra trong quá trình lấy file link data 1: file không tồn tại hoặc không có dữ liệu trong file hoặc có lỗi trong quá trình đọc file ", "Error Get File Link Data 1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    case Model.ResultAddAddress.ERROR_GET_LINK_2:
                        MessageBox.Show("Lỗi xảy ra trong quá trình lấy file link data 2: file không tồn tại hoặc không có dữ liệu trong file hoặc có lỗi trong quá trình đọc file ", "Error Get File Link Data 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    default:
                        MessageBox.Show("Có lỗi xảy ra trong quá trình link với file address", "Error Add Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                this.updateLable("Xóa bỏ đi giá trị không phù hợp");
                resultAddAddress = Function.MyFunction.ActionFineRemove(ref listDataOut);
                switch (resultAddAddress)
                {
                    case Model.ResultActionFineRemove.OK:
                        break;
                    case Model.ResultActionFineRemove.INFOR_EMPTY:
                        MessageBox.Show("Không cặp linh kiện thay thế nào thỏa mãn!", "Info Remove Final", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    default:
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình thực hiện hành động: " + resultAddAddress, "Error Final Remove", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                this.updateLable("Lấy dữ liệu comment");
                //Add Comment
                Function.MyFunction.AddComment(ref this.listDataOut, this.valueInput);
                Function.MyFunction.MainRomveDupplicate(ref this.listDataOut);
                this.GetInfo();//Thuc hien lay ten

                this.dgvResult.DataSource = listDataOut;
                this.SetHeader();//Thiet lap Header cho dgv

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi xảy ra trong quá trình chạy chương trình liên hệ bộ phận IT để được hỗ trợ: " + ex.Message, "Error Program", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.actionButton(true);//Sau cung trong moi hanh dong la enable hanh dong
            }


        }

        #region Action Select file

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
                        case Model.TypeClick.FILE_WO_FIRST:
                            //Thuc hien clear du lieu ban dau
                            this.ClearDataFirst();
                            this.ClearDataInput();//Clear du lieu bien tam
                            this.ClearDataResult();

                            this.txtFileWOFirst.Text = tempResult;
                            this.txtFileWOFirst.SelectionStart = tempResult.Length;//Select vao vi tri cuoi cung
                            break;
                    }
                    break;
            }
        }
        private void btnSelectFileWOFirst_Click(object sender, EventArgs e)
        {
            this.ClickSelectFile(TypeClick.FILE_WO_FIRST);
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
            this.valueInput.file_ETSD1 = this.txtFileETSD1.Text;
            this.valueInput.file_ETSD2 = this.txtFileETSD2.Text;
            this.valueInput.fileLinkData1 = this.txtFileLinkData1.Text;
            this.valueInput.fileLinkData2 = this.txtFileLinkData2.Text;
        }

        private bool ValidateInput()
        {
            this.GetInput();
            string resultValidate = Function.MyFunction.CheckInput(ref this.valueInput);
            switch (resultValidate)
            {
                case Model.ResultCheckInput.OK:
                    return true;
                case Model.ResultCheckInput.ERROR_EMPTY:
                    MessageBox.Show("Không được để trống các ô textbox !!!", "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_DUPLICATE_FILE:
                    MessageBox.Show("Đường dẫn các file trùng lặp hãy kiểm tra lại các ô textbox!", "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_1:
                    MessageBox.Show("File 1 không tồn tại:" + valueInput.file_1, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_2:
                    MessageBox.Show("File 2 không tồn tại:" + valueInput.file_2, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_ETSD_1:
                    MessageBox.Show("File ETSD 1 không tồn tại:" + valueInput.file_ETSD1, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_ETSD_2:
                    MessageBox.Show("File ETSD 2 không tồn tại:" + valueInput.file_ETSD2, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_LINK_DATA_1:
                    MessageBox.Show("File Link data 1 không tồn tại:" + valueInput.fileLinkData1, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_EXIST_FILE_LINK_DATA_2:
                    MessageBox.Show("File Link data 2 không tồn tại:" + valueInput.fileLinkData2, "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                case Model.ResultCheckInput.ERROR_NOT_CSV:
                    MessageBox.Show("Các file phải đúng định dạng .csv!!", "Error Check Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
           
            this.ActiveControl = this.txtFileWOFirst;
            this.txtFileWOFirst.Text = @"P:\96. Share Data\99. Other\13. IT\HOAI\SMT-Compare_A\TEST\XRHP06733.csv";






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

        /// <summary>
        /// Thuc hien thiet lap header cho dgv
        /// </summary>
        /// CreatedBy: HoaiPT(06/02/2023)
        private void SetHeader()
        {
            this.dgvResult.Columns[0].HeaderText = "San xuat " + '\n' + this.getInfo.wo1;
            this.dgvResult.Columns[1].HeaderText = "Vi tri 1";
            this.dgvResult.Columns[2].HeaderText = "Ghi chu 1";
            this.dgvResult.Columns[3].HeaderText = "San xuat tiep theo " + '\n' + this.getInfo.wo2;
            this.dgvResult.Columns[4].HeaderText = "Vi tri 2";
            this.dgvResult.Columns[5].HeaderText = "Ghi chu 2";
            this.dgvResult.Columns[6].HeaderText = "Item Main";

            this.dgvResult.Columns[0].Width = 150;
            this.dgvResult.Columns[1].Width = 60;
            this.dgvResult.Columns[2].Width = 30;
            this.dgvResult.Columns[3].Width = 150;
            this.dgvResult.Columns[4].Width = 60;
            this.dgvResult.Columns[5].Width = 30;
            this.dgvResult.Columns[6].Width = 160;

        }

        #region Action Child Right
        /// <summary>
        /// Thuc hien hanh dong khi click vao nut Search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// CreatedBy: HoaiPT(04/02/2023)
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (this.listDataOut.Count > 0)
            {
                string tempValue = this.txtSearch.Text.Trim();
                var listTemp = this.listDataOut.Where(x => x.col_2.Contains(tempValue) || x.col_1.Contains(tempValue)).ToList();
                this.dgvResult.DataSource = listTemp;
            }


        }

        /// <summary>
        /// Thuc hien click vao nut Fresh trong tim kiem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// CreatedBy: HoaiPT(04/02/2023)
        private void btnSearchFresh_Click(object sender, EventArgs e)
        {

            if (this.listDataOut.Count > 0)
            {
                this.txtSearch.Text = "";
                this.dgvResult.DataSource = this.listDataOut;
                this.SetHeader();
            }

        }

        /// <summary>
        /// Hanh dong khi an nut keydown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnSearch.PerformClick();
            }
        }

        /// <summary>
        /// Hanh dong khi click vao nut Export CSV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// CreatedBy: HoaiPT(04/02/2023)
        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                this.actionButton(false);
                this.updateLable("Thực hiện bắt đầu ghi dữ liệu");

                if (listDataOut.Count == 0)//Kiem tra khi co du lieu thi moi duoc export
                {
                    MessageBox.Show("Không có dữ liệu để export!", "Error No data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        string pathFile = fbd.SelectedPath;

                        var allLines = (from item in listDataOut
                                        select new object[]
                                        {
                                            item.col_1,
                                            item.address_1,
                                            item.comment_1,
                                            item.col_2,
                                            item.address_2,
                                            item.comment_2,
                                        }).ToList();

                        var csv = new StringBuilder();
                        string tempAddString = "\"PICKLIST 1\"" + ",\"" + this.getInfo.wo1 + "\",\"" + this.getInfo.model1 + "\"";
                        csv.AppendLine(tempAddString);
                        tempAddString = "\"PICKLIST 2\"" + ",\"" + this.getInfo.wo2 + "\",\"" + this.getInfo.model2 + "\"";
                        csv.AppendLine(tempAddString);
                        csv.AppendLine();

                        string clientHeader = "\"" + this.getInfo.wo1 + "\"" + ",\"" + "Vi tri 1" + "\",\"" + "Ghi chu 1" + "\",\"" +
                                               this.getInfo.wo2 + "\",\"" + "Vi tri 2" + "\",\"" + "Ghi chu 2" + "\"";
                        csv.AppendLine(clientHeader);
                        allLines.ForEach(line =>
                        {
                            csv.AppendLine(string.Join(",", line));
                        });

                        string fileName = pathFile + @"\" + this.getInfo.wo1 + "_" + this.getInfo.wo2 + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv";
                        File.WriteAllText(fileName, csv.ToString());

                        MessageBox.Show("Thực hiện export dữ liệu thành công file:" + fileName, "Successful Export Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                //Thuc hien lay du lieu

            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi xảy ra trong quá trình chạy chương trình liên hệ bộ phận IT để được hỗ trợ: " + ex.Message, "Error Program", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.actionButton(true);
            }
        }

        private void GetInfo()
        {
            this.getInfo.wo1 = this.lblWO1.Text.Substring(0, this.lblWO1.Text.IndexOf(":") - 1);
            this.getInfo.wo2 = this.lblWO2.Text.Substring(0, this.lblWO2.Text.IndexOf(":") - 1);
            this.getInfo.model1 = this.lblWO1.Text.Substring(this.lblWO1.Text.IndexOf(":") + 1);
            this.getInfo.model2 = this.lblWO2.Text.Substring(this.lblWO2.Text.IndexOf(":") + 1);
        }




        #endregion

        /// <summary>
        /// Hanh dong moi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// CreatedBy: HoaiPT(20/02/2023)
        private void btnLinkFile_Click(object sender, EventArgs e)
        {
            try
            {


                this.actionButton(false);
                this.updateLable("Thực hiện link file...");
                //Thuc hien clear du lieu ban dau
                this.ClearDataFirst();
                this.ClearDataInput();//Clear du lieu bien tam

                string resultValidate = Function.MyFunction2.CheckWOFirst(this.txtFileWOFirst.Text, ref this.valueInput);
                if (!resultValidate.Equals(MdlCommon.OK))
                {
                    MessageBox.Show(resultValidate, "Error Check Link File Auto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.txtFile1.Text = this.valueInput.file_1;
                this.txtFile1.SelectionStart = this.valueInput.file_1.Length;

                this.txtFile2.Text = this.valueInput.file_2;
                this.txtFile2.SelectionStart = this.valueInput.file_2.Length;

                this.txtFileETSD1.Text = this.valueInput.file_ETSD1;
                this.txtFileETSD1.SelectionStart = this.valueInput.file_ETSD1.Length;

                this.txtFileETSD2.Text = this.valueInput.file_ETSD2;
                this.txtFileETSD2.SelectionStart = this.valueInput.file_ETSD2.Length;

                this.txtFileLinkData1.Text = this.valueInput.fileLinkData1;
                this.txtFileLinkData1.SelectionStart = this.valueInput.fileLinkData1.Length;

                this.txtFileLinkData2.Text = this.valueInput.fileLinkData2;
                this.txtFileLinkData2.SelectionStart = this.valueInput.fileLinkData2.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi xảy ra trong quá trình chạy chương trình liên hệ bộ phận IT để được hỗ trợ: " + ex.Message, "Error Program", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.actionButton(true);
            }

        }
        private void ClearDataInput()
        {
            this.valueInput.fileLinkData1 = null;
            this.valueInput.fileLinkData2 = null;
            this.valueInput.file_1 = null;
            this.valueInput.file_2 = null;
            this.valueInput.file_ETSD1 = null;
            this.valueInput.file_ETSD2 = null;
        }
    }
}
