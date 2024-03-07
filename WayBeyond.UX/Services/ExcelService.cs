using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Task<List<Debtor>> ReadClientFile(Client client, FileObject file)
        {
            var debtors = new List<Debtor>();
            _xlWrkBks = _xlApp.Workbooks;
            _xlWrkBk = _xlWrkBks.Open(file.FullPath);
            _xlWrkSht = _xlWrkBk.Worksheets["TESTFILE"];

            var debtor = new Debtor();
            foreach(var detail in client.FileFormat.FileFormatDetails)
            {
                var prop = typeof(Debtor).GetProperty(detail.Field);
                prop = _xlWrkSht.Cells[1,detail.FileColumn];
            }

            return Task.FromResult(debtors);
        }
    }
}
