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
            _drop = new DropFileWrite(_db, transfer);
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
                        byte[] data = System.IO.File.ReadAllBytes(extractedFile.FullPath);

                        List<(int start, int length)> lines = new();

                        int recStart = 0;

                        //This defines the start position and length of each field for each record type.
                        int[,] r01 = { { 1, 2 }, { 3, 4 }, { 7, 8 }, { 15, 15 }, { 30, 10 }, { 40, 1 }, { 41, 15 }, { 56, 10 }, { 66, 1 }, { 67, 25 }, { 92, 15 }, { 107, 18 }, { 125, 2 }, { 127, 5 }, { 132, 21 }, { 153, 1 }, { 154, 6 }, { 160, 8 }, { 168, 10 }, { 178, 7 }, { 185, 10 }, { 195, 1 }, { 196, 2 }, { 198, 10 }, { 208, 16 }, { 224, 9 }, { 233, 10 }, { 243, 30 }, { 273, 3 }, { 276, 26 }, { 302, 5 }, { 307, 25 }, { 332, 59 }, { 391, 50 } };
                        int[,] r02 = { { 1, 2 }, { 3, 1 }, { 4, 25 }, { 29, 25 }, { 54, 25 }, { 79, 253 } };
                        int[,] r03 = { { 1, 2 }, { 3, 1 }, { 4, 6 }, { 10, 2 }, { 15, 3 }, { 18, 7 }, { 25, 9 }, { 34, 9 }, { 43, 23 }, { 66, 12 }, { 78, 30 }, { 108, 5 }, { 113, 24 }, { 137, 25 }, { 162, 13 }, { 175, 20 }, { 195, 20 }, { 215, 3 }, { 218, 25 }, { 243, 5 }, { 248, 25 }, { 273, 25 }, { 298, 25 }, { 323, 9 } };

                        // This loop get's the start and end position of each record. This wouldn't be necessary
                        // if the records were all the same length.
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i] == 0x0A)
                            {
                                int recLen = i - recStart;
                                lines.Add((recStart, recLen));
                                recStart = i + 1;
                            }
                        }

                        foreach(var line in lines)
                        {
                            var recordType = Encoding.ASCII.GetString(data, line.start, 2);

                            switch (recordType)
                            {
                                case "01":
                                    debtor = new AanmaRecords
                                    {
                                        ClientDebtorNumber = Encoding.ASCII.GetString(data, line.start + r01[2,0] - 1, r01[2,1]).Trim(),
                                        PatientsLastName = Encoding.ASCII.GetString(data, line.start + r01[3, 0] - 1, r01[3, 1]).Trim(),
                                        PatientsFirstName = $"{Encoding.ASCII.GetString(data, line.start + r01[4,0]-1, r01[4,1]).Trim()} {Encoding.ASCII.GetString(data, line.start + r01[5, 0] - 1, r01[5, 1]).Trim()}",
                                        DebtorLastName = Encoding.ASCII.GetString(data, line.start + r01[6, 0] - 1, r01[6, 1]).Trim(),
                                        DebtorFirstMiddleName = $"{Encoding.ASCII.GetString(data, line.start + r01[7, 0] - 1, r01[7, 1]).Trim()} {Encoding.ASCII.GetString(data, line.start + r01[8, 0] - 1, r01[8, 1]).Trim()}",
                                        DebtorAddress1 = Encoding.ASCII.GetString(data, line.start + r01[9, 0] - 1, r01[9, 1]).Trim(),
                                        DebtorCity = Encoding.ASCII.GetString(data, line.start + r01[11, 0] - 1, r01[11, 1]).Trim(),
                                        DebtorState = Encoding.ASCII.GetString(data, line.start + r01[12, 0] - 1, r01[12, 1]).Trim(),
                                        DebtorZip = Encoding.ASCII.GetString(data, line.start + r01[13, 0] - 1, r01[13, 1]).Trim(),
                                        DebtorDOB = Encoding.ASCII.GetString(data, line.start + r01[16, 0] - 1, r01[16, 1]).Trim().ToDateTimeMDY(),
                                        PatientsPhone = Encoding.ASCII.GetString(data, line.start + r01[18, 0] - 1, r01[18, 1]).Trim(),
                                        DebtorPhone = Encoding.ASCII.GetString(data, line.start + r01[18, 0] - 1, r01[18, 1]).Trim(),
                                        DebtorEmpPhone = Encoding.ASCII.GetString(data, line.start + r01[20, 0] - 1, r01[20, 1]).Trim(),
                                        DebtorSSN = Encoding.ASCII.GetString(data, line.start + r01[25, 0] - 1, r01[25, 1]).Trim(),
                                        DebtorEmail = Encoding.ASCII.GetString(data, line.start + r01[33, 0] - 1, r01[33, 1]).Trim()

                                    };
                                    debtors.Add(debtor);
                                    break;
                                case "02":
                                    if (debtor != null)
                                    {
                                        debtor.DebtorEmployerName = Encoding.ASCII.GetString(data, line.start + r02[2, 0] - 1, r02[2, 1]).Trim();
                                    }
                                    break;
                                case "03":
                                    if (debtor != null)
                                    {
                                        var trans = new Transaction
                                        {
                                            ServiceDate = Encoding.ASCII.GetString(data, line.start + r03[2, 0] - 1, r03[2, 1]).Trim().ToDateTimeMDY(),
                                            Amount = Encoding.ASCII.GetString(data, line.start + r03[5, 0] - 1, r03[5, 1]).Trim().ToDoubleNoDecimal(),
                                            FacilityNumber = Encoding.ASCII.GetString(data, line.start + r03[17, 0] - 1, r03[17, 1]).Trim(),
                                            FacilityDescription = Encoding.ASCII.GetString(data, line.start + r03[18, 0] - 1, r03[18, 1]).Trim(),
                                            InsuranceName = Encoding.ASCII.GetString(data, line.start + r03[20, 0] - 1, r03[20, 1]).Trim()
                                        };
                                        debtor.Transactions.Add(trans);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            
                        }

                        #region Old Inacurate code

                        //var records = System.IO.File.ReadAllLines(extractedFile.FullPath);
                        //using (TextFieldParser parser = new TextFieldParser(extractedFile.FullPath))
                        //{
                        //    parser.TextFieldType = FieldType.FixedWidth;
                        //    foreach (var record in records)
                        //    {
                        //        switch (record.Substring(0, 2))
                        //        {
                        //            case "01":
                        //                parser.FieldWidths = demoFields;
                        //                var demoData = parser.ReadFields();
                        //                debtor = new AanmaRecords
                        //                {
                        //                    ClientDebtorNumber = demoData[2],
                        //                    PatientsLastName = demoData[3],
                        //                    PatientsFirstName = $"{demoData[4]} {demoData[5]}",
                        //                    DebtorLastName = demoData[6],
                        //                    DebtorFirstMiddleName = $"{demoData[7]} {demoData[8]}",
                        //                    DebtorAddress1 = demoData[9],
                        //                    DebtorCity = demoData[11],
                        //                    DebtorState = demoData[12],
                        //                    DebtorZip = demoData[13],
                        //                    DebtorDOB = demoData[16].ToDateTimeMDY(),
                        //                    PatientsPhone = demoData[18],
                        //                    DebtorPhone = demoData[18],
                        //                    DebtorEmpPhone = demoData[20],
                        //                    DebtorSSN = demoData[25],
                        //                    DebtorEmail = demoData[33]

                        //                };
                        //                debtors.Add(debtor);
                        //                break;
                        //            case "02":
                        //                parser.FieldWidths = empFields;
                        //                var empData = parser.ReadFields();
                        //                if(debtor != null)
                        //                {
                        //                    debtor.DebtorEmployerName = empData[2];
                        //                }
                        //                break;
                        //            case "03":
                        //                parser.FieldWidths = transFields;
                        //                var transData = parser.ReadFields();

                        //                if (debtor != null)
                        //                {
                        //                    var trans = new Transaction
                        //                    {
                        //                        ServiceDate = transData[2].ToDateTimeMDY(),
                        //                        Amount = transData[5].ToDoubleNoDecimal(),
                        //                        FacilityNumber = transData[17],
                        //                        FacilityDescription = transData[18],
                        //                        InsuranceName = transData[20]
                        //                    };
                        //                    debtor.Transactions.Add(trans);
                        //                }
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        //debtors.Add(debtor);
                        //    }
                        //}
                        #endregion
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
        public override DateTime? GetDateOfService() => DateOfService = Transactions.Select(t => t.ServiceDate).Min();
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
