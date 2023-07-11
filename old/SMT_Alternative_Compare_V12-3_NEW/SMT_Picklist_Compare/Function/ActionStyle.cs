using SMT_Picklist_Compare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace SMT_Picklist_Compare.Function
{
    public class ActionStyle
    {
        public static void CheckYInFileExcel(ref Excel.Worksheet ws, List<DataOut> listDataOut)
        {
            int indexCurrent = 5;
            foreach (var item in listDataOut)
            {
                if (item.comment_1 != null)
                {
                    if (item.comment_1.Contains(MdlCommn.CHECKY))
                    {
                        ws.Range[$"A{indexCurrent}:C{indexCurrent}"].Font.Strikethrough = true;
                    }
                }

                if (item.comment_2 != null)
                {
                    if (item.comment_2.Contains(MdlCommn.CHECKY))
                    {
                        ws.Range[$"C{indexCurrent}:F{indexCurrent}"].Font.Strikethrough = true;
                    }
                }
                indexCurrent++;
            }
        }
        public static void SetCheckY(ref System.Windows.Forms.DataGridView dgvResult)
        {

            int rowDgv = dgvResult.Rows.Count;
            for (int i = 0; i < rowDgv; i++)
            {
                if (dgvResult.Rows[i].Cells[2].Value != null) //Thuc hien kiem tra co du lieu khong
                {
                    if (dgvResult.Rows[i].Cells[2].Value.ToString().Contains(MdlCommn.CHECKY))//Kiem tra xem co chua check Y hay khong
                    {
                        dgvResult.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        continue;
                    }
                }
                if (dgvResult.Rows[i].Cells[5].Value != null)//Neu ma tren chua thuc hien duoc thi chuyen sang duoi
                {
                    if (dgvResult.Rows[i].Cells[5].Value.ToString().Contains(MdlCommn.CHECKY))
                    {
                        dgvResult.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                    }
                }
            }

        }
        public static void SetHeader(ref System.Windows.Forms.DataGridView dgvResult, string wo1, string wo2)
        {
            dgvResult.Columns[0].HeaderText = "Dang san xuat " + '\n' + wo1;
            dgvResult.Columns[1].HeaderText = "Vi tri 1";
            dgvResult.Columns[2].HeaderText = "Ghi chu 1";
            dgvResult.Columns[3].HeaderText = "San xuat tiep theo " + '\n' + wo2;
            dgvResult.Columns[4].HeaderText = "Vi tri 2";
            dgvResult.Columns[5].HeaderText = "Ghi chu 2";
            dgvResult.Columns[6].HeaderText = "Item Main";

            //dgvResult.Columns[0].Width = 150;
            //dgvResult.Columns[1].Width = 65;
            //dgvResult.Columns[2].Width = 45;
            //dgvResult.Columns[3].Width = 150;
            //dgvResult.Columns[4].Width = 65;
            //dgvResult.Columns[5].Width = 45;
            //dgvResult.Columns[6].Width = 160;
            dgvResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; // Thiết lập tỷ lệ cột để điều chỉnh kích thước cho đầy đủ không gian hiển thị
            dgvResult.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells); // Tự động điều chỉnh kích thước cột dựa trên nội dung ô dữ liệu

        }

    }
}
