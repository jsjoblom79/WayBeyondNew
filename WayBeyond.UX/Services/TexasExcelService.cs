using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace WayBeyond.UX.Services
{
    public class TexasExcelService
    {
        private Excel.Application xlApp;
        private Excel.Workbook xlWrkBk;
        private Excel.Worksheet xlWrkSht;

        public TexasExcelService()
        {
            xlApp = new Excel.Application();
            xlWrkBk = xlApp.Workbooks.Add();
            xlWrkSht = xlWrkBk.Sheets[1];
            xlApp.DisplayAlerts = false;
            xlApp.Visible = true;
        }
        public void CreatePifDoc(string docName, List<ToPIF> list)
        {
            var fields = typeof(ToPIF).GetProperties();
            int row = 0;
            int col = 1;
            foreach (ToPIF toPIF in list)
            {
                foreach (PropertyInfo field in fields)
                {
                    if (row == 0)
                    {
                        xlWrkSht.Cells[row + 1, col] = field.Name;
                    }
                    xlWrkSht.Cells[row + 2, col] = list[row].GetType().GetProperty(field.Name).GetValue(list[row]);
                    col++;
                }
                col = 1;
                row++;

            }
            xlWrkSht.Cells[row + 2, "F"] = "TOTALS";
            xlWrkSht.Cells[row + 2, "F"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "G"] = $"=SUM(G2:G{row + 1})";
            xlWrkSht.Cells[row + 2, "G"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "H"] = $"=SUM(H2:H{row + 1})";
            xlWrkSht.Cells[row + 2, "H"].Font.Bold = true;
            xlWrkSht.Columns["E:F"].NumberFormat = "MM/dd/yyyy";
            xlWrkSht.Columns["G:H"].NumberFormat = "[$$-en-US] #,##0.00";
            xlWrkSht.Columns.AutoFit();
            xlWrkBk.SaveAs(docName);
            Dispose();
        }
        public void CreateBadDebtReport(string docName, List<ToBadDebt> list)
        {
            var fields = typeof(ToBadDebt).GetProperties();
            int row = 0;
            int col = 1;
            foreach (ToBadDebt debt in list)
            {
                foreach (PropertyInfo field in fields)
                {
                    if (row == 0)
                    {
                        xlWrkSht.Cells[row + 1, col] = field.Name;
                    }
                    xlWrkSht.Cells[row + 2, col] = list[row].GetType().GetProperty(field.Name).GetValue(list[row]);
                    col++;
                }
                col = 1;
                row++;
            }

            xlWrkSht.Cells[row + 2, "G"] = "TOTALS";
            xlWrkSht.Cells[row + 2, "G"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "H"] = $"=SUM(H2:H{row + 1})";
            xlWrkSht.Cells[row + 2, "H"].Font.Bold = true;
            xlWrkSht.Columns["H"].NumberFormat = "[$$-en-US] #,##0.00";
            xlWrkSht.Columns["I:J"].NumberFormat = "MM/dd/yyyy";
            xlWrkSht.Columns.AutoFit();
            xlWrkBk.SaveAs($"{docName}.xlsx");
            Dispose();
        }
        public void CreateCharityReport(string docName, List<ToCharity> list)
        {
            var fields = typeof(ToCharity).GetProperties();
            int row = 0;
            int col = 1;
            foreach (ToCharity debt in list)
            {
                foreach (PropertyInfo field in fields)
                {
                    if (row == 0)
                    {
                        xlWrkSht.Cells[row + 1, col] = field.Name;
                    }
                    xlWrkSht.Cells[row + 2, col] = list[row].GetType().GetProperty(field.Name).GetValue(list[row]);
                    col++;
                }
                col = 1;
                row++;
            }
            xlWrkSht.Cells[row + 2, "G"] = "TOTALS";
            xlWrkSht.Cells[row + 2, "G"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "H"] = $"=SUM(H2:H{row + 1})";
            xlWrkSht.Cells[row + 2, "H"].Font.Bold = true;
            xlWrkSht.Columns["H"].NumberFormat = "[$$-en-US] #,##0.00";
            xlWrkSht.Columns["I:J"].NumberFormat = "MM/dd/yyyy";
            xlWrkSht.Columns.AutoFit();
            xlWrkBk.SaveAs($"{docName}.xlsx");
            Dispose();
        }
        public void CreateInventoryReport(string docName, List<ToInventory> list)
        {
            var fields = typeof(ToInventory).GetProperties();
            int row = 0;
            int col = 1;
            foreach (ToInventory debt in list)
            {
                foreach (PropertyInfo field in fields)
                {
                    if (row == 0)
                    {
                        xlWrkSht.Cells[row + 1, col] = field.Name;
                    }
                    xlWrkSht.Cells[row + 2, col] = list[row].GetType().GetProperty(field.Name).GetValue(list[row]);
                    col++;
                }
                col = 1;
                row++;
            }
            xlWrkSht.Cells[row + 2, "F"] = "TOTALS";
            xlWrkSht.Cells[row + 2, "F"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "G"] = $"=SUM(G2:G{row + 1})";
            xlWrkSht.Cells[row + 2, "G"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "H"] = $"=SUM(H2:H{row + 1})";
            xlWrkSht.Cells[row + 2, "H"].Font.Bold = true;
            xlWrkSht.Columns["E:F"].NumberFormat = "MM/dd/yyyy";
            xlWrkSht.Columns["G:H"].NumberFormat = "[$$-en-US] #,##0.00";
            xlWrkSht.Columns.AutoFit();
            xlWrkBk.SaveAs($"{docName}.xlsx");
            Dispose();
        }
        public void CreateCancelReport(string docName, List<ToCancel> list)
        {
            var fields = typeof(ToCancel).GetProperties();
            int row = 0;
            int col = 1;
            foreach (ToCancel debt in list)
            {
                foreach (PropertyInfo field in fields)
                {
                    if (row == 0)
                    {
                        xlWrkSht.Cells[row + 1, col] = field.Name;
                    }
                    xlWrkSht.Cells[row + 2, col] = list[row].GetType().GetProperty(field.Name).GetValue(list[row]);
                    col++;
                }
                col = 1;
                row++;
            }
            xlWrkSht.Cells[row + 2, "F"] = "TOTALS";
            xlWrkSht.Cells[row + 2, "F"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "G"] = $"=SUM(G2:G{row + 1})";
            xlWrkSht.Cells[row + 2, "G"].Font.Bold = true;
            xlWrkSht.Cells[row + 2, "H"] = $"=SUM(H2:H{row + 1})";
            xlWrkSht.Cells[row + 2, "H"].Font.Bold = true;
            xlWrkSht.Columns["E:F"].NumberFormat = "MM/dd/yyyy";
            xlWrkSht.Columns["G:H"].NumberFormat = "[$$-en-US] #,##0.00";
            xlWrkSht.Columns.AutoFit();
            xlWrkBk.SaveAs($"{docName}.xlsx");
            Dispose();
        }
        public void Dispose()
        {
            xlWrkBk.Close(false);
            xlApp.Quit();
        }
    }
}
