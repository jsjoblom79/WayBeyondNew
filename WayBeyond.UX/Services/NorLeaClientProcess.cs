using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;


namespace WayBeyond.UX.Services
{
    public class NorLeaClientProcess : IEpicClientProcess
    {
        private IBeyondRepository _db;
        private ITransfer _transfer;
        private DropFileWrite _dropFileWrite;
        public NorLeaClientProcess(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;

            _transfer = transfer;
            _dropFileWrite = new(db, transfer);

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

        public async Task<bool> ProcessEpicClientAsync(FileObject file, Client client)
        {
            //Download the file to process.
            var dlFile = await _transfer.DownloadFileAsync(file);

            var parts = dlFile.FileName.Split("_");

            switch (parts[5].ToLower())
            {
                case "inventory":
                    break;
                case "update":
                    break;
                case "withdrawl":
                    break;
                case "assignement":
                    var debtors = ProcessAssignmentFile(dlFile);
                    var batch = await _db.GetCurrentBatch();
                    await WriteDropFileAsync(client,debtors,batch);
                    return await CreateClientLoadAsync(client,debtors,batch, dlFile);
                default:
                    return false;
            }
            return false;
        }

        public Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
            return Task.FromResult(_dropFileWrite.WriteDropFile(client,debtors,batch));
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
                ClientName = DetermineClient(fields[16], fields[69].ToPayType()),
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
        #endregion


    }
   
}
