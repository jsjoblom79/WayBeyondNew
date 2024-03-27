using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WayBeyond.Data.Models;
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
            }
            else
            {
                _xlWrkSht = _xlWrkBk.Sheets.Item[_xlWrkBk.Sheets.Count];
            }

            _xlWrkSht.Columns.AutoFit();
            CleanUpEmptyRows(client.FileFormat.ColumnForClientDebtorNumber);
            var rows = _xlWrkSht.UsedRange.Rows.Count;

            for (int? row = client.FileFormat.FileStartLine; row < rows + 1; row++)
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
                                    debtor.GetType().GetProperty(detail.Field).SetValue(debtor, _xlWrkSht.Cells[row, detail.FileColumn].Text);
                                }
                                break;
                            case "double":
                                debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToDouble());
                                break;
                            case "DateTime":
                                debtor.GetType().GetProperty(detail.Field).SetValue(debtor, ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).ToDateTime());
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
                        
                    } catch (Exception ex)
                    {
                        Log.Error(ex.StackTrace);
                        MessageBox.Show(ex.StackTrace);
                    }
                }
                debtors.Add(debtor);
                OnUpdate($"Debtor: {debtor.ClientDebtorNumber} created.");
            }
            Close();
            return Task.FromResult(debtors);
        }

        private object GetSpecialCase(FileFormatDetail detail, int? row)
        {
            switch (detail.SpecialCase)
            {
                case "Split,2":
                    if (string.IsNullOrWhiteSpace(_xlWrkSht.Cells[row, detail.FileColumn].Text)) return null;
                    
                    var split2 = ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).Split(',');
                    return split2[1];
                case "Split,1":
                    if (string.IsNullOrWhiteSpace(_xlWrkSht.Cells[row, detail.FileColumn].Text)) return null;
                    var split1 = ((string)_xlWrkSht.Cells[row, detail.FileColumn].Text).Split(',');
                    return split1[0];
               
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

        private void Close()
        {
            Marshal.FinalReleaseComObject(_xlWrkSht);
            _xlWrkBk.Close();
            Marshal.FinalReleaseComObject(_xlWrkBk);
            _xlWrkBks.Close();
            Marshal.FinalReleaseComObject(_xlWrkBks);
            _xlApp.Quit();

            Marshal.FinalReleaseComObject(_xlApp);
            

        }
    }
}
