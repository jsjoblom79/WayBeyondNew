using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WayBeyond.Data.Models;
using Serilog;
using Unity;

namespace WayBeyond.UX.Services
{
    public class AanmaClientProcess : IEpicClientProcess
    {
        private IBeyondRepository _db;
        private ITransfer _transfer;
        public event Action<string> ProcessUpdates;
        private DropFileWrite _drop;
        public AanmaClientProcess(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;

            _transfer = transfer;
            _drop = new DropFileWrite(_db);
        }
        public async Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            return await _drop.CreateClientLoad(client, debtors, batch, file);
        }

        public async Task<ProcessedFileBatch> GetBatchFileAsync()
        {
            return await _db.GetCurrentBatch();
        }

        public async Task<bool> ProcessEpicClientAsync(FileObject file, Client client)
        {
            int[] demoFields = new int[] { 2, 4, 8, 15, 10, 1, 15, 10, 1, 25, 15, 18, 2, 5, 21, 1, 6, 8, 10, 7, 10, 1, 2, 10, 16, 9, 10, 30, 3, 26, 5, 25,59,50 };
            int[] empFields = new int[] { 2,1,25,25,25,253 };
            int[] transFields = new int[] { 2,1,6,5,3,7,9,9,23,12,30,5,24,25,13,20,20,3,25,5,25,25,25,9};

            client = await _db.GetClientByClientIdAsync(982);
            try
            {
                //Download file
                var dlFile = await _transfer.DownloadFileAsync(file);
                //Extract the file from zip
                ExtractFiles(dlFile);
                var debtors = new List<Debtor>();
                AanmaRecords debtor = null;
                var fileObjects = new List<FileObject>();
                foreach (var item in await _db.GetFileLocationsByNameAsync(LocationName.Placements))
                {
                    fileObjects.AddRange(await _transfer.GetFileObjectsAsync(item));
                }
                
                foreach(var extractedFile in fileObjects)
                {
                    if (extractedFile.FileName.StartsWith("ELEC"))
                    {
                        var records = System.IO.File.ReadAllLines(extractedFile.FullPath);
                        using (TextFieldParser parser = new TextFieldParser(extractedFile.FullPath))
                        {
                            parser.TextFieldType = FieldType.FixedWidth;
                            foreach (var record in records)
                            {
                                switch (record.Substring(0, 2))
                                {
                                    case "01":
                                        parser.FieldWidths = demoFields;
                                        var demoData = parser.ReadFields();
                                        debtor = new AanmaRecords
                                        {
                                            ClientDebtorNumber = demoData[2],
                                            PatientsLastName = demoData[3],
                                            PatientsFirstName = $"{demoData[4]} {demoData[5]}",
                                            DebtorLastName = demoData[6],
                                            DebtorFirstMiddleName = $"{demoData[7]} {demoData[8]}",
                                            DebtorAddress1 = demoData[9],
                                            DebtorCity = demoData[11],
                                            DebtorState = demoData[12],
                                            DebtorZip = demoData[13],
                                            DebtorDOB = demoData[16].ToDateTimeMDY(),
                                            PatientsPhone = demoData[18],
                                            DebtorPhone = demoData[18],
                                            DebtorEmpPhone = demoData[20],
                                            DebtorSSN = demoData[25],
                                            DebtorEmail = demoData[33]

                                        };
                                        debtors.Add(debtor);
                                        break;
                                    case "02":
                                        parser.FieldWidths = empFields;
                                        var empData = parser.ReadFields();
                                        if(debtor != null)
                                        {
                                            debtor.DebtorEmployerName = empData[2];
                                        }
                                        break;
                                    case "03":
                                        parser.FieldWidths = transFields;
                                        var transData = parser.ReadFields();

                                        if (debtor != null)
                                        {
                                            var trans = new Transaction
                                            {
                                                ServiceDate = transData[2].ToDateTimeMDY(),
                                                Amount = transData[5].ToDoubleNoDecimal(),
                                                FacilityNumber = transData[17],
                                                FacilityDescription = transData[18],
                                                InsuranceName = transData[20]
                                            };
                                            debtor.Transactions.Add(trans);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                //debtors.Add(debtor);
                            }
                        }
                        if (await WriteDropFileAsync(client, debtors, await GetBatchFileAsync(), dlFile))
                            if (await CreateClientLoadAsync(client, debtors, await GetBatchFileAsync(), dlFile))
                                if (await _transfer.ArchiveFileAsync(dlFile))
                                    await _transfer.ArchiveFileAsync(file);

                    }
                }
                

                
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public Task<bool> ProcessEpicClientAsync(FileObject file, Client[]? client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            return _drop.WriteDropFile(client, debtors, batch);
        }

        private bool ExtractFiles(FileObject file)
        {
            try
            {
                if (Path.GetExtension(file.FileName).Equals(".zip"))
                {
                    ZipFile.ExtractToDirectory(file.FullPath, Path.GetDirectoryName(file.FullPath));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                
            }
        }
    }

    public class AanmaRecords : Debtor
    {
        public AanmaRecords()
        {
            Transactions = new List<Transaction>();
        }
        public List<Transaction>  Transactions { get; set; }
        //public new override double AmountReferred  = Math.Round(Transactions.Sum(t => t.Amount), 2);//{ get { Math.Round(Transactions.Sum(t => t.Amount), 2); } protected set; }
        public override double GetAmountReferred() => AmountReferred = Math.Round(Transactions.Sum(t => t.Amount),2);
        public override DateTime? GetDateOfService() => DateOfService = Transactions.Select(t => t.ServiceDate).Max();
        public override string? GetPatientMiscData1() => PatientMiscData1 = Transactions.GroupBy(t => t.FacilityDescription)
            .Select(t => t.First()).FirstOrDefault().FacilityDescription;
        public override string? GetPatientMiscData2() => PatientMiscData2 = Transactions.GroupBy(t => t.FacilityNumber)
            .Select(t => t.First()).FirstOrDefault().FacilityNumber;
        public override string? GetInsuranceName() => InsuranceName = Transactions.GroupBy(t => t.InsuranceName)
            .Select(t => t.First()).FirstOrDefault().InsuranceName;
    }
    public class Transaction
    {
        public DateTime? ServiceDate { get; set; }
        public string TransactionCode { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public string FacilityNumber { get; set; }
        public string FacilityDescription { get; set; }
        public string InsuranceName { get; set; }
        public string GroupNumber { get; set; }


    }
}
