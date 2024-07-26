using Microsoft.Office.Interop.Excel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WayBeyond.Data.Models;
using WayBeyond.UX.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace WayBeyond.UX.Services
{
    public class ExcelService
    {
        private Excel.Application _xlApp;
        private Excel.Workbook _xlWrkBk;
        private Excel.Workbooks _xlWrkBks;
        private Excel.Worksheet _xlWrkSht;

        public ExcelService()
        {
            _xlApp = new Excel.Application();
            _xlApp.DisplayAlerts = false;
            _xlApp.Visible = true;
        }
        public event Action<string> Update = delegate { };
        private void OnUpdate(string message)
        {
            Update(message);
        }
        #region ReadingFiles
        public Task<List<Debtor>> ReadClientFile(Client client, FileObject file)
        {
            var debtors = new List<Debtor>();
            _xlWrkBks = _xlApp.Workbooks;
            _xlWrkBk = _xlWrkBks.Open(file.FullPath);
            if(_xlWrkBk.Sheets.Count > 1)
            {
                var list = new List<string>();
                foreach (Excel.Worksheet item in _xlWrkBk.Sheets)
                {
                    list.Add(item.Name);
                }
                var ShtFrm = new WorksheetsFrm(list);
                if(ShtFrm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _xlWrkSht = _xlWrkBk.Sheets.Item[ShtFrm.WorkSheetName];
                }
            }
            else
            {
                _xlWrkSht = _xlWrkBk.Sheets.Item[_xlWrkBk.Sheets.Count];
            }

            _xlWrkSht.Columns.AutoFit();
            //CleanUpEmptyRows(client.FileFormat.ColumnForClientDebtorNumber);
            var rows = _xlWrkSht.UsedRange.Rows.Count;

            for (int? row = client.FileFormat.FileStartLine; row < rows + 1; row++)
            {
                if (!IsRowEmpty(_xlWrkSht, row, client.FileFormat.ColumnForClientDebtorNumber))
                {
                    var debtor = new Debtor();
                    foreach (var detail in client.FileFormat.FileFormatDetails)
                    {
                        try
                        {

                            switch (detail.ColumnType)
                            {
                                case "string":
                                    if (detail.SpecialCase != null)
                                    {

                                        debtor.GetType().GetProperty(detail.Field).SetValue(debtor, GetSpecialCase(detail, row));
                                    }
                                    else
                                    {
                                        if (detail.FileColumn.Contains(","))
                                        {
                                            var columns = detail.FileColumn.Split(',');
                                            debtor.GetType().GetProperty(detail.Field).SetValue(debtor, $"{((string)_xlWrkSht.Cells[row, columns[0]].Text).ToCleanString()} " +
                                                $"{((string)_xlWrkSht.Cells[row, columns[1].Trim()].Text).ToCleanString()}");
                                        }
                                        else
                                        {
                                            debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToCleanString());
                                        }

                                    }
                                    break;
                                case "double":
                                    debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToDouble());
                                    break;
                                case "DateTime":
                                    if (detail.SpecialCase != null && detail.SpecialCase == SpecialCase.MultiDate)
                                    {
                                        if (detail.FileColumn.Contains(","))
                                        {
                                            var columns = detail.FileColumn.Split(',');
                                            List<string> dates = new List<string>();
                                            foreach (var item in columns)
                                            {
                                                dates.Add(_xlWrkSht.Cells[row, item].Text);
                                            }
                                            debtor.GetType().GetProperty(detail.Field).SetValue(debtor, dates.ToMaxDateFromList());
                                        }
                                        else
                                        {
                                            debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToSingleDateFromMulti());
                                        }

                                    }
                                    else
                                    {
                                        debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToDateTime());
                                    }
                                    break;
                                case "long":
                                    debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToLong());
                                    break;
                                case "int":
                                    debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToInt());
                                    break;
                                case "Zip":
                                    debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToZip());
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.StackTrace);
                            MessageBox.Show(ex.StackTrace);
                        }
                    }
                    debtors.Add(debtor);
                    OnUpdate($"Debtor: {debtor.ClientDebtorNumber} created.");
                }
            }
                Close();
            return Task.FromResult(debtors);
        }

        private bool IsRowEmpty(Worksheet xlwrksht, int? row, string col)
        {
            Excel.Range cell = xlwrksht.Cells[row,col] as Excel.Range;

            bool isEmpty = cell.Value2 == null || string.IsNullOrEmpty(cell.Text);
            Marshal.ReleaseComObject(cell);
            return isEmpty;
            
        }

        private object? GetSpecialCase(FileFormatDetail detail, int? row)
        {
            switch (detail.SpecialCase)
            {
                case SpecialCase.Split2Comma:
                    if (string.IsNullOrWhiteSpace(_xlWrkSht.Cells[row, detail.FileColumn].Text)) return null;

                    var split2 = ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).Split(',');
                    return split2[1];
                case SpecialCase.Split1Comma:
                    if (string.IsNullOrWhiteSpace(_xlWrkSht.Cells[row, detail.FileColumn].Text)) return null;
                    var split1 = ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).Split(',');
                    return split1[0];
                case SpecialCase.Split1:
                    if (string.IsNullOrWhiteSpace(_xlWrkSht.Cells[row, detail.FileColumn].Text)) return null;
                    var sp1 = ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).Split(" ");
                    return sp1[0].ToCleanString();
                case SpecialCase.Split2:
                    if (string.IsNullOrWhiteSpace(_xlWrkSht.Cells[row, detail.FileColumn].Text)) return null;
                    var sp2 = ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).Split(" ");
                    return sp2[1].ToCleanString();
                default:
                    return null;
            }
        }
        private void CleanUpEmptyRows(string columnIndex)
        {
            try
            {
                var rows = _xlWrkSht.UsedRange.Rows.Count;

               
                    if (((string)_xlWrkSht.Cells[rows, columnIndex].Text).Equals(""))
                    {
                        _xlWrkSht.Columns[columnIndex].SpecialCells(Excel.XlCellType.xlCellTypeBlanks).EntireRow.Delete();
                    }
               

                
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace, ex);
            }
        }
        #endregion
        #region WritingClientLoadFile
        public Task<bool> WriteClientLoadFile(List<ClientLoad> loads, string filename)
        {
            try
            {
                _xlWrkBks = _xlApp.Workbooks;
                _xlWrkBk = _xlApp.Workbooks.Add();
                _xlWrkSht = _xlWrkBk.Worksheets["Sheet1"];

                var totalBalance = loads.Sum(l => l.Balance);
                var totalAccounts = loads.Sum(l => l.DebtorCount);

                var adjustedLoads = from load in loads
                                    select new { ClientId = load.ClientId, ClientName = load.ClientName, Balance = load.Balance, DebtorCount = load.DebtorCount };

                var fields = new[] { "ClientId", "ClientName", "Balance", "DebtorCount" };
                var row = 1;
                var col = 1;
                Excel.Range topRow = _xlWrkSht.Range[_xlWrkSht.Cells[1, 1], _xlWrkSht.Cells[1, fields.Count()]];
                Excel.Range totalRow = _xlWrkSht.Range[_xlWrkSht.Cells[adjustedLoads.Count() + 2, 1], _xlWrkSht.Cells[adjustedLoads.Count() + 2, fields.Count()]];
                foreach ( var field in fields )
                {
                    _xlWrkSht.Cells[row, col] = System.Text.RegularExpressions.Regex.Replace(field,"(\\B[A-Z])"," $1");
                    col++;
                }
                row = 2;
                foreach(var load in adjustedLoads)
                {
                    col = 1;
                    //handles the information for the columns
                    foreach(var field in fields)
                    {
                        _xlWrkSht.Cells[row, col] = load.GetType().GetProperty(field).GetValue(load);
                        col++;
                    }
                    row++;
                }
                _xlWrkSht.Cells[row, "C"] = totalBalance;
                _xlWrkSht.Cells[row, "D"] = totalAccounts;
                _xlWrkSht.Rows[row].Font.Bold = true;
                topRow.Font.Bold = true;
                topRow.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;

                totalRow.Font.Bold = true;
                totalRow.Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
                _xlWrkSht.Columns["C"].NumberFormat = "$ ###,##0.00";
                _xlWrkSht.Columns.AutoFit();
                
                _xlWrkBk.SaveAs($"{filename}.xlsx");
                Close();
                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
                Close();
                return Task.FromResult(false);
            }      
        }
        #endregion

        public async Task WriteExcelFiles(IEnumerable<object> records, string filename)
        {
            try
            {
                _xlWrkBks = _xlApp.Workbooks;
                _xlWrkBk = _xlApp.Workbooks.Add();
                _xlWrkSht = _xlWrkBk.Worksheets["Sheet1"];

                var fields = records.GetType().GetProperties();
                var info = fields[2].PropertyType.FullName;
                var objs = info.GetType().GetProperties();
                var row = 1;
                var col = 1;
                foreach ( var field in objs )
                {
                    _xlWrkSht.Cells[row, col] = field.Name;
                    col++;
                }
                row++;
                col = 1;
                foreach( var record in records)
                {
                    var props = record.GetType().GetProperties();
                    foreach(var field in fields)
                    {
                        _xlWrkSht.Cells[row, col] = record.GetType().GetProperty(field.Name).GetValue(record);
                        col++;
                    }
                    row++;
                }
                _xlWrkBk.SaveAs($"{filename}.xlsx");
                Close();

            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
                Close();
                
            }
            return;
        }
        private void Close()
        {
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_xlWrkSht);
            _xlWrkSht = null;
            _xlWrkBk.Close();
            Marshal.FinalReleaseComObject(_xlWrkBk);
            _xlWrkBk = null;
            _xlWrkBks.Close();
            Marshal.FinalReleaseComObject(_xlWrkBks);
            _xlWrkBks = null;
            _xlApp.Quit();
            Marshal.FinalReleaseComObject(_xlApp);
            _xlApp = null;

        }
    }
}
