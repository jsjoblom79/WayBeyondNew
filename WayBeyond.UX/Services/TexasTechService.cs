using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class TexasTechService
    {
        private ITTRepo _repo;
        private List<TexasDebtor> debtors = new List<TexasDebtor>();
        private List<Account> accounts = new List<Account>();
        private List<Patient> patients = new List<Patient>();
        private List<TUResult> TUResults = new List<TUResult>();
        
        public string ReportMonth
        {
            get
            {
                return GetReportMonth();
            }
        }

        public string ReportYear
        {
            get
            {
                return GetReportYear();
            }
        }
        public TexasTechService(ITTRepo repo)
        {
            _repo = repo;
           
        }
        public void TruncateTables()
        {
            _repo.TruncateTables();

        }

        public void CreateFolders(string folderPath)
        {
            //string folderPath = $"{}{GetReportMonth()} {GetReportYear()}";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        #region Table Updates


        public void UpdateTUResults()
        {
            var tus = TUResults.GroupBy(d => d.MRN).Select(d => d.First()).ToList();
            _repo.CreateTUResult(tus);
            _repo.UpdateScode();
        }
        public void UpdateDatabase()
        {
            var debt = debtors.GroupBy(d => d.Mrn).Select(d => d.First()).ToList();
            var acct = accounts.GroupBy(d => d.Inv).Select(d => d.First()).ToList();
            var pat = patients.GroupBy(d => d.Mrn).Select(d => d.First()).ToList();
            _repo.CreateDebtors(debt);
            _repo.CreateAccounts(acct);
            _repo.CreatePatients(pat);
            _repo.InsertPayments();
            _repo.InsertDOR();

        }
        public void UpdateExpiredAccounts()
        {
            _repo.InsertExpiredAccounts();
        }
        public void ReadInActiveRecords(string fileName)
        {
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;

                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();
                    if (fields[0].Equals("Registration FSC 1"))
                    {
                        //skip
                    }
                    else
                    {
                        debtors.Add(GetDebtor(fields));
                        accounts.Add(GetAccount(fields, false));
                        patients.Add(GetPatient(fields));
                    }
                }


            }
        }
        public void ReadActiveRecords(string fileName)
        {


            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;


                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();
                    if (fields[0].Equals("Registration FSC 1"))
                    {
                        //skip
                    }
                    else
                    {
                        debtors.Add(GetDebtor(fields));
                        accounts.Add(GetAccount(fields, true));
                        patients.Add(GetPatient(fields));
                    }
                }

            }
        }

        private Patient GetPatient(string[] fields)
        {
            return new Patient
            {

                FirstName = fields[2].ToFirstName(),
                LastName = fields[2].ToLastName(),
                Dob = fields[5].ToDate(),
                Ssn = fields[6],
                AddressLine1 = fields[7],
                City = fields[8],
                State = fields[9],
                Zip = fields[10],
                Mrn = fields[3].ToMrn()
            };
        }

        private Account GetAccount(string[] fields, bool active)
        {
            return new Account
            {
                RegistrationFsc1 = fields[0],
                RegistrationFsc2 = fields[1],
                ClientDebtorNumber = fields[3],
                AccountNumber = fields[4],
                Balance = fields[12].ToDouble(),
                ServiceDate = fields[13].ToDate(),
                DateLastPay = fields[24].ToDate(),
                StatusCode = fields[25],
                DateOfReferral = fields[26].ToDate(),
                AmountReferred = fields[27].ToDouble(),
                CloseDate = fields[28].ToDate(),
                MRN = fields[3].ToMrn(),
                Inv = fields[3].ToInv(),
                Active = active
            };
        }

        private TexasDebtor GetDebtor(string[] fields)
        {
            return new TexasDebtor
            {

                Telephone = fields[11],
                Employer = fields[14],
                LastName = fields[15],
                FirstName = fields[16],
                Ssn = fields[17],
                Dob = fields[18].ToDate(),
                AddressLine1 = fields[19],
                City = fields[20],
                State = fields[21],
                Zip = fields[22],
                MessageTelephone = fields[23],
                Mrn = fields[3].ToMrn()
            };
        }

        public void UpdateScode(string fileName)
        {
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;

                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();

                    if (!fields[0].Equals("Registration FSC 1"))
                    {
                        TUResults.Add(new TUResult
                        {
                            MRN = fields[2],
                            Scode = fields[38]
                        });
                    }
                }
            }
        }
        #endregion
        public void GetPIFList(string reportFolder)
        {
            var list = _repo.GetPIF();
            TexasExcelService excel = new();
            excel.CreatePifDoc($"{reportFolder}TT_PIF_{GetReportMonth()}_{GetReportYear()}", list);
        }
        public void GetBadDebtList(string reportFolder)
        {
            var list = _repo.GetBadDebts();
            TexasExcelService excel = new();
            excel.CreateBadDebtReport($"{reportFolder}TT_BAD_DEBT_{GetReportMonth()}_{GetReportYear()}", list);
        }
        public void GetCharityList(string reportFolder)
        {
            var list = _repo.GetCharities();
            TexasExcelService excel = new();
            excel.CreateCharityReport($"{reportFolder}TT_Charity_{GetReportMonth()}_{GetReportYear()}", list);
        }
        public void GetCancelList(string reportFolder)
        {
            var list = _repo.GetCancels();
            TexasExcelService excel = new();
            excel.CreateCancelReport($"{reportFolder}TT_Cancel_{GetReportMonth()}_{GetReportYear()}", list);
        }
        public void GetInventoryList(string reportFolder)
        {
            var list = _repo.GetInventories();
            TexasExcelService excel = new();
            excel.CreateInventoryReport($"{reportFolder}TT_Active_Inventory_{GetReportMonth()}_{GetReportYear()}", list);
        }
        private string GetReportYear()
        {
            if (DateTime.Now.Month == 1)
            {
                return $"{DateTime.Now.AddYears(-1).Year}";
            }
            else
            {
                return $"{DateTime.Now.Year}";
            }
        }
        private string GetReportMonth()
        {
            return $"{DateTime.Now.AddMonths(-1):MMMM}";
        }
        public void GetTransunionList(string filename)
        {
            //DateTime.TryParse(timeframe, out var date);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("RegistrationFSC1,RegistrationFSC2,PatientLastName,PatientFirstName,MRN,Acct#,DOB,SSN,AddressLine1,City,State,ZIP,GuarTelephone#,Inv #,Amount,ServDt,GuarEmployer,GuarantorLastName,GuarantorFirstName,GuarSSN,GuarDOB,GuarAddress,GuarCity,GuarState,GuarZIP,MessageTelephone");
            foreach (ToTransunion record in _repo.GetTransunionList())
            {
                sb.AppendLine(record.ToString());
            }

            System.IO.File.WriteAllText(filename, sb.ToString());

        }
    }
}
