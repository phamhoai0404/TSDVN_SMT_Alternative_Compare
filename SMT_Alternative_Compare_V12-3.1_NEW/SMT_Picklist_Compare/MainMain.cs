﻿using SMT_Picklist_Compare.Model;
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
using Excel = Microsoft.Office.Interop.Excel;

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
            this.listDataOut.Clear();
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

                //Thuc hien lay du lieu cua PickList
                List<Picklist> picklist_1 = new List<Picklist>();
                List<Picklist> picklist_2 = new List<Picklist>();
                Function.ActionPicklist.GetDataCSV(this.valueInput.file_1, ref picklist_1, ref this.lblWO1);
                Function.ActionPicklist.GetDataCSV(this.valueInput.file_2, ref picklist_2, ref this.lblWO2);

                //Thuc hien lay du lieu cua LKTT
                List<LKTT_ETSD> listLKTT_1 = new List<LKTT_ETSD>();
                List<LKTT_ETSD> listLKTT_2 = new List<LKTT_ETSD>();
                Function.ActionLKTT.GetDataCSV(this.valueInput.file_ETSD1, ref listLKTT_1);
                Function.ActionLKTT.GetDataCSV(this.valueInput.file_ETSD2, ref listLKTT_2);
                List<LKTT_ETSD> listLKTT_ALL = new List<LKTT_ETSD>();
                listLKTT_ALL = listLKTT_1.Concat(listLKTT_2).ToList();
                Function.ActionLKTT.RemoveDupplicate(ref listLKTT_ALL);

                this.updateLable("Thực hiện xóa cặp item giống nhau trong 2 file");// giá trị giống nhau nhưng không tồn tại 
                List<Picklist> pickList1_After = new List<Picklist>();//Du lieu dung de luu tru khi xoa giong nhau
                List<Picklist> pickList2_After = new List<Picklist>();
                Function.ActionPicklist.CompareTwoFile(picklist_1, picklist_2, listLKTT_ALL, ref pickList1_After, ref pickList2_After);

               
                this.updateLable("Thực hiện so sánh ");
                Function.ActionMain.ActionCompareETSD(pickList1_After, picklist_2, listLKTT_ALL, ref this.listDataOut);

                List<Feeder> listFeeder_1 = new List<Feeder>();
                List<Feeder> listFeeder_2 = new List<Feeder>();
                Function.ActionFeeder.GetDataCSV(this.valueInput.fileLinkData1, ref listFeeder_1);
                Function.ActionFeeder.GetDataCSV(this.valueInput.fileLinkData2, ref listFeeder_2);
                this.updateLable("Thực hiện kiểm tra xuat hien 2 EL");
                Function.ActionCheckEL.GetAddressEL(ref this.listDataOut,listLKTT_ALL, listFeeder_1, listFeeder_2);

                this.updateLable("Thực hiện add comment");
                Function.ActionPicklist.AddComment(ref this.listDataOut, picklist_1, picklist_2, listLKTT_1, listLKTT_2);
                
                if(this.listDataOut.Count == 0)
                {
                    MessageBox.Show("Không có cặp link kiện thay thế!","Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Function.ActionFeeder.AddAddress(ref this.listDataOut, listFeeder_1, listFeeder_2, listLKTT_ALL);

                this.GetInfo();//Thuc hien lay ten
                this.dgvResult.DataSource = listDataOut;
                this.SetStyleDgv();//Thiet lap css cho dgv
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
        private void SetStyleDgv()
        {
            //Thuc hien set du lieu cua Header
            Function.ActionStyle.SetHeader(ref this.dgvResult, this.getInfo.wo1, this.getInfo.wo2);

            //Thuc hien style cho item chua Y
            Function.ActionStyle.SetCheckY(ref this.dgvResult);//Thiet lap to mau Item co tich Y

        }
        #endregion



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
                var listTemp = this.listDataOut.Where(x => (x.col_2?.Contains(tempValue) ?? false) || (x.col_1?.Contains(tempValue) ?? false) ||
                                                            (x.address_1?.Contains(tempValue) ?? false) || (x.address_2?.Contains(tempValue) ?? false) ||
                                                            (x.comment_1?.Contains(tempValue) ?? false) || (x.comment_2?.Contains(tempValue) ?? false) || (x.tempMain?.Contains(tempValue) ?? false)).ToList();
                this.dgvResult.DataSource = listTemp;

                this.SetStyleDgv();//Thuc hien thiet lap cua dgv
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
                this.SetStyleDgv();//Thuc hien style css
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
        private void btnExportCSV_V1_Click(object sender, EventArgs e)
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
        private void btnExportCSV_V2_Click(object sender, EventArgs e)
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
                string fileName = "";
                this.ActionCreateFile(ref fileName);

                MessageBox.Show("Thực hiện export dữ liệu thành công file:" + fileName, "Successful Export Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void ChangeNameFile(string valueOld, string valueNew)
        {
            this.txtFile1.Text = this.valueInput.file_1.Replace(valueOld, valueNew);
            this.txtFileETSD1.Text = this.valueInput.file_ETSD1.Replace(valueOld, valueNew);
            this.txtFileLinkData1.Text = this.valueInput.fileLinkData1.Replace(valueOld, valueNew);

            this.txtFile2.Text = this.valueInput.file_2.Replace(valueOld, valueNew);
            this.txtFileETSD2.Text = this.valueInput.file_ETSD2.Replace(valueOld, valueNew);
            this.txtFileLinkData2.Text = this.valueInput.fileLinkData2.Replace(valueOld, valueNew);

            this.valueInput.file_1 = this.valueInput.file_1.Replace(valueOld, valueNew);
            this.valueInput.file_2 = this.valueInput.file_2.Replace(valueOld, valueNew);
            this.valueInput.file_ETSD1 = this.valueInput.file_ETSD1.Replace(valueOld, valueNew);
            this.valueInput.file_ETSD2 = this.valueInput.file_ETSD2.Replace(valueOld, valueNew);
            this.valueInput.fileLinkData1 = this.valueInput.fileLinkData1.Replace(valueOld, valueNew);
            this.valueInput.fileLinkData2 = this.valueInput.fileLinkData2.Replace(valueOld, valueNew);
        }
        #endregion

        #region Link File
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
        #endregion

        #region Function Other
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.actionButton(false);
                this.updateLable("Thực hiện bắt đầu ghi dữ liệu");

                if (listDataOut.Count == 0)//Kiem tra khi co du lieu thi moi duoc export
                {
                    MessageBox.Show("Không có dữ liệu để in!", "Error No data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string fileName = "";
                this.ActionCreateFile(ref fileName);

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Open(fileName);


                Excel.Worksheet worksheet = workbook.Sheets[1];
                Excel.Range columnsToAutofit = worksheet.Range["A:F"];
                columnsToAutofit.EntireColumn.AutoFit();

                int rowData = this.listDataOut.Count();

                Excel.Range range = worksheet.Range["A4:F" + (4 + rowData)];
                Excel.Borders borders = range.Borders;
                borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                borders.Weight = 2d;

                Function.ActionStyle.CheckYInFileExcel(ref worksheet, this.listDataOut);

                worksheet.PageSetup.PrintArea = "A1:" +
                worksheet.Cells[worksheet.UsedRange.Rows.Count, worksheet.UsedRange.Columns.Count].Address;

                worksheet.PageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;
                worksheet.PageSetup.FitToPagesWide = 1;
                worksheet.PageSetup.FitToPagesTall = 1;



                worksheet.PrintOut(Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);


                workbook.Close(false, Type.Missing, Type.Missing);
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                MessageBox.Show("Thực hiện tạo lệnh in và tạo file thành công :" + fileName, "Successful Print Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void ActionCreateFile(ref string fileName)
        {
            string pathFolder = Path.GetDirectoryName(this.valueInput.file_1) + @"\KET_QUA";
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            //pathFolder = Path.GetDirectoryName(Path.GetDirectoryName(this.valueInput.file_1));

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
            foreach (var line in allLines)
            {
                if (line[0] == null)
                {
                    line[0] = "";
                }
                csv.AppendLine(string.Join(",", line));

            }

            //allLines.ForEach(line =>
            //{
            //    csv.AppendLine(string.Join(",", line));
            //});

            string tempFile = this.getInfo.wo1 + "_" + this.getInfo.wo2 + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            fileName = pathFolder + @"\" + tempFile + ".csv";
            File.WriteAllText(fileName, csv.ToString());

            string pathFolderOld = Path.GetDirectoryName(this.valueInput.file_1);
            pathFolder = Path.GetDirectoryName(pathFolderOld);


            try
            {
                System.IO.Directory.Move(pathFolderOld, pathFolder + @"\" + tempFile);
                fileName = pathFolder + @"\" + tempFile + @"\KET_QUA\" + tempFile + ".csv";
                this.ChangeNameFile(pathFolderOld, pathFolder + @"\" + tempFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Folder đang mở bởi hoạt động khác => Không đổi được tên thư mục! " + ex.Message, "Successful Export Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
