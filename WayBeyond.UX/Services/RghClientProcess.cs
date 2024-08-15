using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace WayBeyond.UX.Services
{
    public class RghClientProcess : IEpicClientProcess
    {
        public event Action<string> ProcessUpdates;
        private IBeyondRepository _db;
        private ITransfer _transfer;
        private Client _medicare;
        private Client _nonMedicare;
        private List<Debtor> _debtors;
        private FileObject _fileObject;
        private DropFileWrite _drop;
        private string[] _fileContents;
        public RghClientProcess(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;
            _transfer = transfer;
            _medicare = _db.GetClientByClientIdAsync(1348).Result;
            _nonMedicare = _db.GetClientByClientIdAsync(1081).Result;
            _drop = new DropFileWrite(_db);
        }
        public async Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            throw new NotImplementedException();
        }

        public async Task<ProcessedFileBatch> GetBatchFileAsync()
        {
            return await _db.GetCurrentBatch();
        }

        public async Task<bool> ProcessEpicClientAsync(FileObject file, Client client)
        {
            //download the file
            _fileObject = file;
            _debtors = new List<Debtor>();
            var dlFile = await _transfer.DownloadFileAsync(file);

            _fileContents = System.IO.File.ReadAllLines(dlFile.FullPath);
            foreach (var line in _fileContents)
            {
                var records = line.Split('*');
                if (records[0].Equals("20A"))
                {
                    var debtor = new Debtor
                    {
                        ClientDebtorNumber = records[1].Trim(),
                        DebtorLastName = records[2].ToLastNameFirst().Trim(),
                        DebtorFirstMiddleName = records[2].ToFirstMiddleLast().Trim(),
                        DebtorSSN = records[3].ToCleanString().Trim(),
                        DebtorAddress1 = records[4].Trim(),
                        DebtorAddress2 = records[5].Trim(),
                        DebtorCity = records[6].Trim(),
                        DebtorState = records[7].Trim(),
                        DebtorZip = records[8].Trim(),
                        DebtorPhone = records[9].ToCleanString().Trim(),
                        DebtorEmail = records[10].Trim(),
                        DebtorDOB = records[13].Trim().ToDateTimeYMD()
                    };
                    _debtors.Add(debtor);
                }
            }

            foreach (var line in _fileContents)
            {
                var records = line.Split("*");
                var debtor = _debtors.Where(d => d.ClientDebtorNumber == records[1]).FirstOrDefault();

                switch (records[0])
                {
                    case "10":
                        debtor.PatientsLastName = records[3].Trim().ToLastNameFirst();
                        debtor.PatientsFirstName = records[3].Trim().ToFirstMiddleLast();
                        debtor.PatientsSSN = records[5].ToCleanString().Trim();
                        debtor.PatientAddress = $"{records[6].Trim()} {records[7].Trim()}";
                        debtor.PatientCity = records[8].Trim();
                        debtor.PatientState = records[9].Trim();
                        debtor.PatientsPhone = records[12].ToCleanString().Trim();
                        debtor.PatientMiscData1 = records[13].Trim();
                        debtor.PatientsDOB = records[15].Trim().ToDateTimeYMD();
                        break;
                    case "25":
                        debtor.PatientsEmployer = records[2].Trim();
                        debtor.PatientEmpPhone = records[3].ToCleanString().Trim();
                        break;
                    case "26":
                        debtor.DebtorEmployerName = records[2].Trim();
                        debtor.DebtorEmpPhone = records[3].ToCleanString().Trim();
                        break;
                    case "35":
                        debtor.AmountReferred = records[6].Trim().ToDouble();
                        break;
                    case "30":
                        debtor.DateOfService = records[6].Trim().ToDateTimeYMD();
                        debtor.Location = records[11].Trim();
                        if (records[4].Trim().ToUpper().Equals("MEDICARE"))
                        {
                            debtor.IsMedicare = true;
                        }
                        break;
                    case "55":
                        debtor.BillingNumber = records[3].Trim();
                        debtor.RGHRecordStatus = records[2].Trim();
                        break;
                    case "56":
                        debtor.TransactionNumber = records[2].Trim();
                        break;
                    case "60A":
                        debtor.InsuranceName = records[4].Trim();
                        debtor.InsurancePhone = records[10].ToCleanString().Trim();
                        break;
                }
            }
            await _transfer.ArchiveFileAsync(dlFile);
            return await CreateDropRecords();

        }

        private async Task<bool> CreateDropRecords()
        {
            var medicareAccounts = _debtors.Where(d => d.IsMedicare == true && d.RGHRecordStatus.Equals("A")).ToList();
            var nonMedicareAccounts = _debtors.Where(d => d.IsMedicare == false && d.RGHRecordStatus.Equals("A")).ToList();
            var filesProcessed = false;

            var batch = await GetBatchFileAsync();

            if (await WriteDropFileAsync(_medicare, medicareAccounts, batch, null))
            {
                filesProcessed = true;
            }


            if (await WriteDropFileAsync(_nonMedicare, nonMedicareAccounts, batch, null))
            {
                filesProcessed = true;
            }



            if (filesProcessed)
            {
                filesProcessed = await _transfer.ArchiveFileAsync(_fileObject);
                WriteDeleteFile();
                WriteUpdate();
            }
            return filesProcessed;
        }

        public async Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            if (_drop.WriteDropFile(client, debtors, batch))
            {
                return await _drop.CreateClientLoad(client, debtors, batch, _fileObject);
            }
            else { return false; }
        }

        private async Task WriteUpdate()
        {
            List<RghUpdateRec> debtors = new List<RghUpdateRec>();


            foreach (var line in _fileContents)
            {
                var records = line.Split('*');
                if (records[0].Equals("20A"))
                {
                    debtors.Add(new RghUpdateRec
                    {
                        ClientDebtorNumber = records[1].Trim(),
                        DebtorFirstMiddleName = records[2].Trim().ToFirstMiddleLast(),
                        DebtorLastName = records[2].Trim().ToLastNameFirst()
                    });
                }
            }

            foreach (var line in _fileContents)
            {
                var records = line.Split('*');
                var debtor = debtors.Where(d => d.ClientDebtorNumber == records[1]).FirstOrDefault();

                switch (records[0])
                {
                    case "35":
                        debtor.AmountReferred = records[6].Trim().ToDouble();
                        debtor.DateLastPay = records[7].Trim().ToDateTimeYMD();
                        break;
                    case "55":
                        debtor.CurrentBalance = records[5].ToDouble(); //Current Balance
                        debtor.RghRecordStatus = records[2].Trim();
                        break;
                    case "70":
                        debtor.Transactions.Add(new RghTransactions { ClientDebtorNumber = records[1].Trim(), BatchDate = records[11].Trim().ToDateTimeYMD(), CodeType = records[6].Trim(), CodeDescription = records[4].Trim(), Amount = records[7].Trim().ToDouble() });
                        break;
                }
            }

            var results = debtors.Where(d => d.RghRecordStatus == "U").ToList();


            var xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            xlApp.Visible = true;

            var xlWrkBks = xlApp.Workbooks;
            var xlWrkBk = xlApp.Workbooks.Add();
            var xlWrkSht = xlWrkBk.Worksheets["Sheet1"];

            var fields = results[0].GetType().GetProperties();
            var row = 1;
            var col = 1;
            foreach (var field in fields)
            {
                xlWrkSht.Cells[row, col] = field.Name;
                var range = xlWrkSht.Cells[row, col] as Excel.Range;
                range.Interior.Color = Excel.XlRgbColor.rgbGreen;
                col++;
            }
            row++;
            col = 1;
            foreach (var record in results)
            {
                            
                foreach (var field in fields)
                {
                    xlWrkSht.Cells[row, col] = record.GetType().GetProperty(field.Name).GetValue(record);
                    col++;
                }
                row++; col = 2;
                var transFields = record.Transactions[0].GetType().GetProperties();
                foreach (var field in transFields)
                {
                    xlWrkSht.Cells[row,col] = field.Name;
                    var range = xlWrkSht.Cells[row, col] as Excel.Range;
                    range.Interior.Color = Excel.XlRgbColor.rgbAliceBlue;
                    col++;
                }
                row++; col = 2;
                foreach(var trans in record.Transactions)
                {
                    foreach(var field in transFields)
                    {
                        xlWrkSht.Cells[row, col] = trans.GetType().GetProperty(field.Name).GetValue(trans);
                        col++;
                    }
                    row++; col = 2;
                }
                col = 1;
                row++;
            }
            var output = await _db.GetSingleFileLocationByNameAsync(LocationName.EpicPlacementOutput);
            var filename = $"{output.Path}RGH_Updates_{DateTime.Now:yyyyMMdd}.xlsx";
            xlWrkBk.SaveAs(filename);
            xlWrkSht = null;
            xlWrkBk.Close();
            Marshal.FinalReleaseComObject(xlWrkBk);
            xlWrkBk = null;
            xlWrkBks.Close();
            Marshal.FinalReleaseComObject(xlWrkBks);
            xlWrkBks = null;
            xlApp.Quit();
            Marshal.FinalReleaseComObject(xlApp);
            xlApp = null;

        }
        private async Task WriteDeleteFile()
        {
            List<RghDeleteRec> deleteRecords = new List<RghDeleteRec>();
            foreach (var line in _fileContents)
            {
                var records = line.Split('*');
                if (records[0].Equals("20A"))
                {
                    deleteRecords.Add(new RghDeleteRec
                    {
                        ClientDebtorNumber = records[1].Trim(),
                        DebtorFirstMiddleName = records[2].ToFirstMiddleLast().Trim(),
                        DebtorLastName = records[2].ToLastNameFirst().Trim()
                    });
                }

            }

            foreach (var line in _fileContents)
            {
                var records = line.Split('*');
                var debtor = deleteRecords.Where(d => d.ClientDebtorNumber == records[1]).FirstOrDefault();

                switch (records[0])
                {
                    case "35":
                        debtor.AmountReferred = records[6].Trim().ToDouble();
                        debtor.DateLastPay = records[7].Trim().ToDateTimeYMD();
                        break;
                    case "55":
                        debtor.RghRecordStatus = records[2].Trim();
                        break;

                }
            }

            var printDeletes = deleteRecords.Where(d => d.RghRecordStatus.Equals("D")).ToList();

            var xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            xlApp.Visible = true;

            var xlWrkBks = xlApp.Workbooks;
            var xlWrkBk = xlApp.Workbooks.Add();
            var xlWrkSht = xlWrkBk.Worksheets["Sheet1"];

            var fields = printDeletes[0].GetType().GetProperties();
            var row = 1;
            var col = 1;
            foreach (var field in fields)
            {
                xlWrkSht.Cells[row, col] = field.Name;
                col++;
            }
            row++;
            col = 1;
            foreach (var record in printDeletes)
            {
                var props = record.GetType().GetProperties();
                foreach (var field in fields)
                {
                    xlWrkSht.Cells[row, col] = record.GetType().GetProperty(field.Name).GetValue(record);
                    col++;
                }
                col = 1;
                row++;
            }
            var output = await _db.GetSingleFileLocationByNameAsync(LocationName.EpicPlacementOutput);
            var filename = $"{output.Path}RGH_DELETES_{DateTime.Now:yyyyMMdd}.xlsx";
            xlWrkBk.SaveAs(filename);
            //await service.WriteExcelFiles(printDeletes, );

            xlWrkSht = null;
            xlWrkBk.Close();
            Marshal.FinalReleaseComObject(xlWrkBk);
            xlWrkBk = null;
            xlWrkBks.Close();
            Marshal.FinalReleaseComObject(xlWrkBks);
            xlWrkBks = null;
            xlApp.Quit();
            Marshal.FinalReleaseComObject(xlApp);
            xlApp = null;

        }

        public Task<bool> ProcessEpicClientAsync(FileObject file, Client[]? client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProcessClientFile(FileObject file, Client client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
            throw new NotImplementedException();
        }
    }

    public class RghDeleteRec
    {
        public string? ClientDebtorNumber { get; set; }
        public string? DebtorFirstMiddleName { get; set; }
        public string? DebtorLastName { get; set; }
        public double? AmountReferred { get; set; }
        public DateTime? DateLastPay { get; set; }
        public string? RghRecordStatus { get; set; }
    }
    public class RghUpdateRec : RghDeleteRec
    {
        public RghUpdateRec()
        {
            Transactions = new List<RghTransactions>(); 
        }
        public double? CurrentBalance { get; set; }
        public List<RghTransactions> Transactions;
    }
    public class RghTransactions
    {
        public string? ClientDebtorNumber { get; set; }
        public DateTime? BatchDate { get; set; }
        public string? CodeType { get; set; }
        public string? CodeDescription { get; set; }
        public double? Amount { get; set; }
    }
}

