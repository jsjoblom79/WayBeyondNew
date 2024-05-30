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
        public FarmingtonFireClientProcess(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;

            _transfer = transfer;

        }
        public Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedFileBatch> GetBatchFileAsync()
        {
            _db.get
        }

        public Task<bool> ProcessEpicClientAsync(FileObject file, Client client)
        {
            var lines = System.IO.File.ReadAllLines(file.FullPath);
            Debtor debtor = null;
            List<Debtor> debtorList = new List<Debtor>();
            foreach (var line in lines)
            {
                var fields = line.Split("*");

                switch (fields[0])
                {
                    case "01":
                        if (debtor != null)
                        {
                            debtor = new Debtor
                            {
                                DebtorFirstMiddleName = $"{fields[2]} {fields[3]}",
                                DebtorLastName = fields[1],
                                DebtorAddress1 = fields[4],
                                DebtorAddress2 = fields[5],
                                DebtorCity = fields[6],
                                DebtorState = fields[7],
                                DebtorZip = fields[8],
                                DebtorPhone = fields[9],
                                DebtorDOB = fields[10].ToDateTime(),
                                DebtorSSN = fields[11],
                                DebtorEmployerName = fields[12],
                                PatientsRelationship = fields[13]
                            };
                        }
                        break;
                    case "02":
                        if (debtor != null)
                        {
                            debtor.ClientDebtorNumber = fields[1];
                            debtor.PatientPaid = fields[2].ToDouble();
                            debtor.PatientPaidDate = fields[3].ToDateTime();
                            debtor.DateOfService = fields[4].ToDateTime();
                            debtor.AmountReferred = fields[5].ToDouble();
                        }
                        break;
                    case "03":
                        if(debtor != null)
                        {
                            if(debtor.PatientsRelationship.ToInt() != 1)
                            {
                                debtor.PatientsLastName = fields[1];
                                debtor.PatientsFirstName = $"{fields[2]} {fields[3]}";
                                debtor.PatientsPhone = fields[9];
                                debtor.PatientsDOB = fields[10].ToDateTime();
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
                        debtor = null;
                        break;
                    default:
                        break;
                }
            }

            return Task.FromResult(false);
        }

        public Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
            throw new NotImplementedException();
        }
    }
}
