using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.Data.Context;

namespace WayBeyond.UX.Services
{
    public class TTRepo : ITTRepo
    {
        private BeyondContext _context;
        public TTRepo()
        {
            _context = new BeyondContext();
        }
        public void CreateAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void CreateAccounts(List<Account> accounts)
        {
            _context.Accounts.AddRange(accounts);
            _context.SaveChanges();
        }

        public void CreateDebtor(TexasDebtor debtor)
        {
            _context.TexasDebtors.Add(debtor);
            _context.SaveChanges();
        }

        public void CreateDebtors(List<TexasDebtor> debtors)
        {
            _context.TexasDebtors.AddRange(debtors);
            _context.SaveChanges();
        }

        public void CreatePatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void CreatePatients(List<Patient> patients)
        {
            _context.Patients.AddRange(patients);
            _context.SaveChanges();
        }

        public void CreateTUResult(List<TUResult> results)
        {
            _context.TUResults.AddRange(results);
            _context.SaveChanges();
        }

        public Account? GetAccount(string inv)
        {
            Account? acct = _context.Accounts.Where(a => a.Inv == inv).FirstOrDefault();
            if (acct != null)
            {
                return acct;
            }
            else
            {
                Console.WriteLine("Unable to Locate Account.");
                return acct;
            }
        }

        public IEnumerable<Account> GetAccounts(string MRN)
        {
            var accounts = _context.Accounts.Where(a => a.MRN == MRN);
            if (accounts.Any())
            {
                return accounts;
            }
            else { return Enumerable.Empty<Account>(); }
        }

        public List<ToBadDebt> GetBadDebts()
        {
            string query = "SELECT a.INV, a.MRN, c.LastName as PLST, c.FirstName as PFIRST, b.LastName as GLST, b.FirstName as GFIRST, a.AccountNumber as OURAcct, a.AmountReferred as Amount, a.ServiceDate as SERVD, " +
                        "b.DOB, a.Scode FROM Account as a INNER JOIN TexasDebtor as B on b.MRN = a.MRN LEFT JOIN Patient as c on c.MRN = a.MRN " +
                        "WHERE a.Active = 1 and (a.Scode = 'Over 200 FPL' or a.AccountNumber IN (SELECT AccountNumber FROM ExpiredAccounts));";
            List<ToBadDebt> list = _context.FromSqlRaw<ToBadDebt>(query).ToList();
            return list;
        }

        public List<ToCancel> GetCancels()
        {
            string query = "SELECT a.MRN , a.INV as Invoice, a.AccountNumber as AcctNum,FirstName || ' ' || LastName as GURNAME, a.DateOfReferral as DOR, a.DateLastPay as DLP,a.AmountReferred as PFC, a.Balance as BAL " +
                          "from Account as a INNER JOIN TexasDebtor as b on b.MRN = a.MRN WHERE StatusCode = 'CCR' and date(CloseDate)>= date('now','start of month','-1 months')";
            List<ToCancel> list = _context.FromSqlRaw<ToCancel>(query).ToList();
            return list;
        }

        public List<ToCharity> GetCharities()
        {
            string query = "SELECT a.INV, a.MRN, c.LastName as PLST, c.FirstName as PFIRST, b.LastName as GLST, b.FirstName as GFIRST, a.AccountNumber as OURAcct, a.AmountReferred as Amount, a.ServiceDate as SERVD, " +
                        "b.DOB, a.Scode FROM Account as a INNER JOIN TexasDebtor as B on b.MRN = a.MRN LEFT JOIN Patient as c on c.MRN = a.MRN " +
                        "WHERE a.Active = 1 and a.Scode = '0 to 200 FPL';";
            List<ToCharity> list = _context.FromSqlRaw<ToCharity>(query).ToList();
            return list;
        }

        public TexasDebtor? GetDebtor(string mrn)
        {
            TexasDebtor? debtor = _context.TexasDebtors.Where(d => d.Mrn == mrn).FirstOrDefault();
            if (debtor != null)
            {
                return debtor;
            }
            else
            {
                Console.WriteLine("Unable to locate debtor.");
                return debtor;
            }
        }

        public IEnumerable<TexasDebtor> GetDebtors()
        {
            return _context.TexasDebtors;
        }

        public List<ToInventory> GetInventories()
        {
            string query = "SELECT a.MRN , a.INV as Invoice, a.AccountNumber as AcctNum,FirstName || ' ' || LastName as GURNAME, a.DateOfReferral as DOR, a.DateLastPay as DLP,a.AmountReferred as PFC, a.Balance as BAL " +
                "FROM Account as a INNER JOIN Texasdebtor as b on b.MRN = a.MRN " +
               " WHERE a.Active = 1 and a.Scode is null and a.AccountNumber not in (SELECT AccountNumber FROM ExpiredAccounts) Order by 5";
            List<ToInventory> list = _context.FromSqlRaw<ToInventory>(query).ToList();
            return list;
        }

        public Patient? GetPatient(string mrn)
        {
            Patient? patient = _context.Patients.Where(p => p.Mrn == mrn).FirstOrDefault();
            if (patient != null)
            {
                return patient;
            }
            else
            {
                Console.WriteLine("Unable to locate patient.");
                return patient;
            }
        }

        public IEnumerable<Patient> GetPatients(string mnr)
        {
            return _context.Patients.Where(p => p.Mrn == mnr);
        }

        public List<ToPIF> GetPIF()
        {
            var query = "SELECT a.MRN as \"MRN\", a.INV as \"INVOICE\",a.AccountNumber as \"AcctNum\",b.FirstName || ' ' || b.LastName as \"NAME\"," +
                "a.DateOfReferral as \"DOR\",a.DateLastPay as \"DLP\",a.AmountReferred as \"PIF\",a.Balance as \"BAL\" FROM Account as a " +
                "INNER JOIN TexasDebtor as b on a.MRN = b.MRN WHERE StatusCode = 'PIF' and date(CloseDate) >= date('now','start of month', '-1 months')";
            List<ToPIF> list = _context.FromSqlRawPif<ToPIF>(query).ToList();
            return list;
        }

        public List<ToTransunion> GetTransunionList()
        {
            var query = "Select a.RegistrationFsc1,a.RegistrationFsc2,p.LastName as PLST,p.FirstName as PFIRST,a.MRN,a.AccountNumber as OURAcct,p.DOB as PDOB,p.SSN as PSSN,p.AddressLine1 as pAddress, " +
                        "p.City as pCity,p.State as pState,p.Zip as PZip,d.Telephone,a.INV,a.Balance,a.ServiceDate,d.Employer,d.LastName as GLST,d.FirstName as GFIRST,d.ssn GSSN, d.DOB GDOB, " +
                        "d.AddressLine1 as Address,d.city,d.State,d.Zip,d.MessageTelephone from Account as a INNER JOIN TexasDebtor as d on d.MRN = a.MRN INNER JOIN Patient as p on p.MRN = a.MRN " +
                        "LEFT JOIN ReferalDate as b on b.MRN = a.MRN LEFT JOIN Payments as c on c.MRN = a.MRN WHERE b.MRN is null and c.MRN is null and active = 1 and StatusCode NOT IN('PPP','PTP','ATY','INS','DSR') " +
                        "GROUP BY a.MRN";


            List<ToTransunion> ttun = _context.FromSqlRaw<ToTransunion>(query).ToList();

            return ttun;
        }

        public void InsertDOR()
        {
            string query = "INSERT INTO ReferalDate (MRN, DateOfReferral) Select MRN, MAX(dateofreferral) FROM Account WHERE Active = 1 and date(DateOfReferral) >= date('now','start of month','-6 months') Group by MRN";
            _context.Database.ExecuteSqlRaw(query);
        }

        public void InsertExpiredAccounts()
        {
            string query = "INSERT INTO ExpiredAccounts (AccountNumber) Select AccountNumber FROM (SELECT a.MRN, a.INV, a.AccountNumber, FirstName || ' ' || LastName as GURNAME, a.DateOfReferral, a.DateLastPay, a.AmountReferred, " +
                "a.Balance, a.StatusCode FROM Account as a Inner JOIN Texasdebtor as b on b.MRN = a.MRN WHERE a.Active = 1 and a.Scode is null) as a LEFT JOIN Payments as b on b.MRN = a.MRN " +
                "WHERE b.MRN is null and date(DateOfReferral) < date('now','start of month','-6 months') and StatusCode not IN ('PPP','PTP','ATY','INS','DSR')";
            _context.Database.ExecuteSqlRaw(query);
        }

        public void InsertPayments()
        {
            var query = "INSERT INTO Payments SELECT MRN, max(DateLastPay) as LastPay FROM Account WHERE date(DateLastPay) >= date('now','start of month','-6 months') GROUP BY MRN";
            _context.Database.ExecuteSqlRaw(query);
            query = "INSERT INTO Status SELECT MRN,'1' From Account WHERE StatusCode IN ('PPP', 'PTP', 'ATY', 'INS') Group by MRN";
            _context.Database.ExecuteSqlRaw(query);
        }

        public void TruncateTables()
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Account; DELETE FROM TexasDebtor; DELETE FROM Patient; DELETE FROM TUResult;" +
                " DELETE FROM Payments; DELETE FROM Status; DELETE FROM ReferalDate; DELETE FROM ExpiredAccounts;");
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void UpdateDebtor(TexasDebtor debtor)
        {
            _context.TexasDebtors.Update(debtor);
            _context.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }

        public void UpdateScode()
        {
            //updates scode
            _context.Database.ExecuteSqlRaw("UPDATE Account SET Scode = (SELECT Scode FROM TUResult WHERE Account.MRN = MRN);");
            //cleans up any scodes that are updated to empty strings.
            _context.Database.ExecuteSqlRaw("UPDATE Account SET Scode = null where Scode = '';");
        }
    }
}
