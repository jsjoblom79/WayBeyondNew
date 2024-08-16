using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class DropFileWrite
    {
        private static IBeyondRepository _db;
        //private static ITransfer _transfer;
        public DropFileWrite(IBeyondRepository db)//, ITransfer transfer)
        {
            _db = db;
            //_transfer = transfer;   
        }
        public bool WriteDropFile(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
            var drop = client.DropFormat;
            var dropDetails = drop.DropFormatDetails;
            var stringBuilder = new StringBuilder();

            try
            {
                //Writes the header record for the Drop File.
                foreach (var detail in dropDetails)
                {
                    stringBuilder.Append(detail.Field);
                    stringBuilder.Append('\t');
                }
                stringBuilder.AppendLine();
                foreach (var debtor in debtors)
                {
                    foreach (var detail in dropDetails)
                    {
                        switch (detail.FieldType)
                        {
                            case "DATE":
                                if (debtor.GetDateOfService() != null)
                                {
                                    if (detail.Field.Equals("DateOfService"))
                                    {
                                        stringBuilder.Append($"{(debtor.GetDateOfService()):MM/dd/yy}");
                                    }
                                    else
                                    {
                                        if(debtor.GetType().GetProperty(detail.Field).GetValue(debtor) != null)
                                            stringBuilder.Append($"{((DateTime)debtor.GetType().GetProperty(detail.Field).GetValue(debtor)):MM/dd/yy}");
                                        stringBuilder.Append("");
                                    }
                                    
                                }
                                    
                                break;
                            case "CURRENCY":
                                if (debtor.GetType().GetProperty(detail.Field).GetValue(debtor) != null)
                                {
                                    if (detail.Field.Equals("AmountReferred"))
                                    {
                                        stringBuilder.Append($"{(debtor.GetAmountReferred()):####.00}");
                                    }
                                    else
                                    {
                                        stringBuilder.Append($"{((double)debtor.GetType().GetProperty(detail.Field).GetValue(debtor)):####.00}");
                                    }
                                }
                                    
                                break;
                            default:
                                switch (detail.Field)
                                {
                                    case "PatientMiscData1":
                                        stringBuilder.Append(debtor.GetPatientMiscData1());
                                        break;
                                    case "PatientMiscData2":
                                        stringBuilder.Append(debtor.GetPatientMiscData2());
                                        break;
                                    case "InsuranceName":
                                        stringBuilder.Append(debtor.GetInsuranceName());
                                        break;
                                    case "DebtorEmpPhone":
                                    case "PatientsSSN":
                                    case "PatientsPhone":
                                    case "InsurancePhone":
                                    case "DebtorPhone":
                                    case "DebtorSSN":
                                    case "SpouseEmpPhone":
                                    case "SpouseSSN":
                                    case "ComakerPhone":
                                    case "DebtorHomePhone":
                                    case "PatientEmpPhone":
                                    case "DebtorCell":
                                        stringBuilder.Append($"{((string)debtor.GetType().GetProperty(detail.Field).GetValue(debtor)).ToCleanString()}");
                                        break;
                                    default:
                                        stringBuilder.Append(((string)debtor.GetType().GetProperty(detail.Field).GetValue(debtor)).RemoveTabs()); 
                                        break;
                                }
                                break;
                        }
                        //stringBuilder.Append(debtor.GetType().GetProperty(detail.Field).GetValue(debtor));
                        stringBuilder.Append('\t');
                    }
                    stringBuilder.AppendLine();
                }
                var path = _db.GetFileLocationsByNameAsync(LocationName.Prepared);
                var fileDateTime = $"{DateTime.Now:yyyyMMdd-HHmmss}";
                System.IO.File.WriteAllText($@"{path.Result[0].Path}{client.ClientId}_{fileDateTime}_{client.DropFileName}", stringBuilder.ToString());
                if (System.IO.File.Exists($@"{path.Result[0].Path}{client.ClientId}_{fileDateTime}_{client.DropFileName}"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace, ex);
                return false;
            }

        }

        public async Task<bool> CreateClientLoad(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            var load = new ClientLoad
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Balance = debtors.Sum(d => d.AmountReferred),
                DebtorCount = debtors.Count(),
                CreateDate = DateTime.Now,
                FileName = file.FileName,
                DateOnLoadFile = file.CreateDate,
                DropNumber = client.DropNumber,
                ProcessedFileBatchId = batch.Id,
                EmailCount = debtors.Where(d => !string.IsNullOrEmpty(d.DebtorEmail)).Count()
            };

            if (await _db.AddClientLoadAsync(load) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
