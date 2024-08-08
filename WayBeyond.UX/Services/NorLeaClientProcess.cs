using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using Excel = Microsoft.Office.Interop.Excel;


namespace WayBeyond.UX.Services
{
    public class NorLeaClientProcess : IEpicClientProcess
    {
        private IBeyondRepository _db;
        private ITransfer _transfer;
        private DropFileWrite _dropFileWrite;
        private const string UpdateFileName = "UPDATE_NOR_LEA_";
        private const string DeleteFileName = "DELETE_NOR_LEA_";
        private DropFormat _dropFormat;
        private Excel.Application _xlApp;
        private Excel.Workbook _xlWrkBk;
        private Excel.Workbooks _xlWrkBks;
        private Excel.Worksheet _xlWrkSht;

        public NorLeaClientProcess(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;

            _transfer = transfer;
            _dropFileWrite = new(db);//, transfer);
            _dropFormat = _db.GetDropFormatByIdAsync(26).Result;
            _xlApp = new Excel.Application();
            _xlApp.DisplayAlerts = false;
            _xlApp.Visible = true;
        }
        public event Action<string> ProcessUpdates = delegate { };

        public Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            return _dropFileWrite.CreateClientLoad(client, debtors, batch, file);
        }

        public async Task<ProcessedFileBatch> GetBatchFileAsync()
        {
            return await _db.GetCurrentBatch();
        }

        public async Task<bool> ProcessEpicClientAsync(FileObject file, Client[] client)
        {
            //Download the file to process.
            var dlFile = await _transfer.DownloadFileAsync(file);

            var parts = dlFile.FileName.Split("_");

            switch (parts[5].ToLower())
            {
                case "inventory":
                    await _transfer.ArchiveFileAsync(dlFile);
                    return await _transfer.ArchiveFileAsync(file);
                case "update":
                    var updateDbtr = ProcessAssignmentFile(dlFile);
                    WriteAdditionalFile(updateDbtr, UpdateFileName);
                    CloseExcel();
                    await _transfer.ArchiveFileAsync(dlFile);
                    return await _transfer.ArchiveFileAsync(file);

                case "withdrawal":
                    var withdrawDbtr = ProcessAssignmentFile(dlFile);
                    WriteAdditionalFile(withdrawDbtr, DeleteFileName);
                    CloseExcel();
                    await _transfer.ArchiveFileAsync(dlFile);
                    return await _transfer.ArchiveFileAsync(file);
                case "assignement":
                    var debtors = ProcessAssignmentFile(dlFile);
                    var batch = await _db.GetCurrentBatch();
                    // This step writes the drop files and adds the loads to the db.
                    await WriteDropFileAsync(null,debtors,batch,dlFile);
                    //Archive Files
                    await _transfer.ArchiveFileAsync(dlFile);
                    return await _transfer.ArchiveFileAsync(file);
                default:
                    return false;
            }
            
        }
        private void CloseExcel()
        {
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
        private async void WriteAdditionalFile(List<Debtor> updateDbtr, string updateFileName)
        {
            _xlWrkBks = _xlApp.Workbooks;
            _xlWrkBk = _xlApp.Workbooks.Add();
            _xlWrkSht = _xlWrkBk.Worksheets["Sheet1"];

            var row = 1;
            var col = 1;
            foreach (var detail in _dropFormat.DropFormatDetails) 
            {
                _xlWrkSht.Cells[row, col] = detail.Field;
                col++;
            }
            row = 2;
            foreach(var record in updateDbtr)
            {
                col = 1;
                foreach(var detail in _dropFormat.DropFormatDetails)
                {
                    _xlWrkSht.Cells[row, col] = record.GetType().GetProperty(detail.Field).GetValue(record);
                    col++;
                }
                row++;
            }

            var output = await _db.GetSingleFileLocationByNameAsync(LocationName.EpicPlacementOutput);
            var filename = $"{output.Path}{updateFileName}{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
            _xlWrkBk.SaveAs(filename);
        }

        public async Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            //group debtors by client
            var groupedByClients = debtors.GroupBy(d => d.Client).ToDictionary(c => c.Key, c => c.ToList());
            var result = false;
            foreach (var group in groupedByClients)
            {
                if (_dropFileWrite.WriteDropFile(group.Key, group.Value, batch))
                {
                    result = await _dropFileWrite.CreateClientLoad(group.Key, group.Value, batch, file);
                }
            }
            return result;
        }


        #region Private Methods
        private List<Debtor> ProcessAssignmentFile(FileObject file)
        {
            List<Debtor> debtors = new();

            string[] lines = System.IO.File.ReadAllLines(file.FullPath);
            foreach (string line in lines)
            {
                string[] fields = line.Split('^');
                if (fields[0].Equals("01"))
                {
                    debtors.Add(GetDebtor(fields));
                }
            }
            return debtors;
        }

        private Debtor GetDebtor(string[] fields)
        {
            return new Debtor
            {
                ClientDebtorNumber = fields[1],
                PatientsFirstName = fields[6],
                PatientsLastName = fields[6],
                PatientsSSN = fields[7],
                PatientsDOB = fields[8].ToDateTime(),
                DateOfService = fields[14].ToDateTime(),
                Client = _db.GetClientByClientId((long)DetermineClient(fields[16], fields[69].ToPayType())),
                PatientsPhone = fields[23],
                DebtorEmail = fields[25],
                PatientMiscData1 = fields[33],
                AmountReferred = fields[72].ToDouble(),
                InsuranceName = fields[76],
                InsurancePolicyNumber = fields[89],
                InsurancePhone = fields[99],
                DebtorFirstMiddleName = fields[151],
                DebtorLastName = fields[151],
                DebtorSSN = fields[152],
                DebtorDOB = fields[153].ToDateTime(),
                DebtorAddress1 = fields[155],
                DebtorAddress2 = fields[156],
                DebtorCity = fields[157],
                DebtorState = fields[158],
                DebtorZip = fields[159],
                DebtorPhone = fields[161],
                DebtorEmpPhone = fields[162],
                DebtorEmployerName = fields[164]
            };
        }

        private ClientId DetermineClient(string clientName, PayType? payType)
        {
            return payType switch
            {
                PayType.MEDICARE => clientName switch
                {
                    "CC NNL EAGLES STUDENT HEALTH CENTER HOBBS" or "CC NNL EAGLES STUDENT HEALTH CENTER" => ClientId.UNIDENTIFIED,
                    "CC NNL TATUM MEDICAL CLINIC" => ClientId.LEA_TATUM_CLINIC_MC,
                    "CC NNL PROFESSIONAL PHYSICIANS CENTER" => ClientId.NOR_LEA_PROF_PHYS_MC,
                    "CC NNL NOR LEA CANCER CENTER" => ClientId.NOR_LEA_CANCER_CENTER_MC,
                    "CC NNL LOVINGTON MEDICAL CLINIC" => ClientId.NOR_LEA_LOVINGTON_CL_MC,
                    "CC NNL LOVING STUDENT HEALTH CENTER" => ClientId.UNIDENTIFIED,
                    "CC NNL HOBBS MEDICAL CLINIC" => ClientId.NOR_LEA_HOBBS_MEICAL_MC,
                    "CC NNL FAMILY HEALTH CENTER LEA COUNTY" => ClientId.NOR_LEA_FAMILY_HEALTH_MC,
                    "CC NNL NOR-LEA HOSPITAL DISTRICT" => ClientId.NOR_LEA_GENERAL_HOSPITAL_MC,
                    _ => ClientId.NOR_LEA_GENERAL_HOSPITAL_MC,
                },
                PayType.NON_MEDICARE => clientName switch
                {
                    "CC NNL EAGLES STUDENT HEALTH CENTER HOBBS" or "CC NNL EAGLES STUDENT HEALTH CENTER" => ClientId.LOVINGTON_STUDENT_CLINIC,
                    "CC NNL TATUM MEDICAL CLINIC" => ClientId.NOR_LEA_TATUM_CLINIC,
                    "CC NNL PROFESSIONAL PHYSICIANS CENTER" => ClientId.NOR_LEA_PROFESSIONAL_PHYS,
                    "CC NNL NOR LEA CANCER CENTER" => ClientId.NOR_LEA_CANCER_CENTER,
                    "CC NNL LOVINGTON MEDICAL CLINIC" => ClientId.NOR_LEA_LOVINGTON_CLINIC,
                    "CC NNL LOVING STUDENT HEALTH CENTER" => ClientId.LOVINGTON_STUDENT_CLINIC,
                    "CC NNL HOBBS MEDICAL CLINIC" => ClientId.NOR_LEA_HOBBS_MEDICAL,
                    "CC NNL FAMILY HEALTH CENTER LEA COUNTY" => ClientId.NOR_LEA_FAMILY_HEALTH,
                    "CC NNL NOR-LEA HOSPITAL DISTRICT" => ClientId.NOR_LEA_GENERAL_HOSPITAL,
                    _ => ClientId.NOR_LEA_GENERAL_HOSPITAL,
                },
                _ => ClientId.UNIDENTIFIED,
            };
        }

        public Task<bool> ProcessEpicClientAsync(FileObject file, Client? client)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
   

}
