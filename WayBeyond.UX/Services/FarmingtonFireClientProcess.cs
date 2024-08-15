using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class FarmingtonFireClientProcess : IEpicClientProcess
    {
        public event Action<string> ProcessUpdates;
        private IBeyondRepository _db;
        private ITransfer _transfer;
        private DropFileWrite _dropFileWrite;
        public FarmingtonFireClientProcess(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;

            _transfer = transfer;
            _dropFileWrite = new DropFileWrite(db);//, transfer);
        }
        public async Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            return await _dropFileWrite.CreateClientLoad(client, debtors, batch, file);
        }

        public Task<ProcessedFileBatch> GetBatchFileAsync()
        {
            return _db.GetCurrentBatch();
        }

        public async Task<bool> ProcessEpicClientAsync(FileObject file, Client client)
        {
            var localFile = await _transfer.DownloadFileAsync(file);
            var batch = await _db.GetCurrentBatch();
            var lines = System.IO.File.ReadAllLines(localFile.FullPath);
            Debtor debtor = new ();
            List<Debtor> debtorList = new List<Debtor>();
            foreach (var line in lines)
            {
                var fields = line.Split("*");
                if (fields[0].Equals("01"))
                {
                    debtor = new Debtor();
                }
                try
                {
                    switch (fields[0])
                    {
                        case "01":
                            if (debtor != null)
                            {

                                debtor.DebtorFirstMiddleName = $"{fields[2]} {fields[3]}";
                                debtor.DebtorLastName = fields[1];
                                debtor.DebtorAddress1 = fields[4];
                                debtor.DebtorAddress2 = fields[5];
                                debtor.DebtorCity = fields[6];
                                debtor.DebtorState = fields[7];
                                debtor.DebtorZip = fields[8];
                                debtor.DebtorPhone = fields[9];
                                debtor.DebtorDOB = fields[10].ToDateTimeMDY();
                                debtor.DebtorSSN = fields[11];
                                debtor.DebtorEmployerName = fields[12];
                                debtor.PatientsRelationship = fields[13];

                            }
                            break;
                        case "02":
                            if (debtor != null)
                            {
                                debtor.ClientDebtorNumber = fields[1];
                                debtor.PatientPaid = fields[2].ToDouble();
                                debtor.PatientPaidDate = fields[3].ToDateTimeMDY();
                                debtor.DateOfService = fields[4].ToDateTimeMDY();
                                debtor.AmountReferred = fields[5].ToDouble();
                            }
                            break;
                        case "03":
                            if (debtor != null)
                            {
                                if (debtor.PatientsRelationship.ToInt() != 1)
                                {
                                    debtor.PatientsLastName = fields[1];
                                    debtor.PatientsFirstName = $"{fields[2]} {fields[3]}";
                                    debtor.PatientsPhone = fields[9];
                                    debtor.PatientsDOB = fields[10].ToDateTimeMDY();
                                    debtor.PatientsSSN = fields[11];
                                }
                            }
                            break;
                        case "04":
                        case "05":
                        case "06":
                        case "07":
                        case "08":
                        case "09":
                            debtorList.Add(debtor);
                            //debtor = null;
                            break;
                        default:
                            break;
                    }
                } catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    return false;
                }
            }

            await WriteDropFileAsync(client, debtorList, batch,null);
            
            var results = await CreateClientLoadAsync(client, debtorList, batch, localFile);
            //Archive Local and remote files
            await _transfer.ArchiveFileAsync(localFile);
            await _transfer.ArchiveFileAsync(file);

            return results;
        }

        public async Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            return await Task.FromResult(_dropFileWrite.WriteDropFile(client, debtors, batch));
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
}
