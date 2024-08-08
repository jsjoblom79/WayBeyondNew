using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public interface ITTRepo
    {
        //Account methods
        void CreateAccount(Account account);
        void CreateAccounts(List<Account> accounts);
        Account? GetAccount(string inv);
        IEnumerable<Account> GetAccounts(string MRN);
        void UpdateAccount(Account account);

        //TUResults
        void CreateTUResult(List<TUResult> results);

        //Debtor Methods
        void CreateDebtor(TexasDebtor debtor);
        void CreateDebtors(List<TexasDebtor> debtors);
        TexasDebtor? GetDebtor(string mrn);
        IEnumerable<TexasDebtor> GetDebtors();
        void UpdateDebtor(TexasDebtor debtor);

        //Patient Methods
        void CreatePatient(Patient patient);
        void CreatePatients(List<Patient> patients);
        Patient? GetPatient(string mrn);
        IEnumerable<Patient> GetPatients(string mnr);
        void UpdatePatient(Patient patient);

        //truncates and updates.
        void TruncateTables();
        void UpdateScode();
        void InsertPayments();
        void InsertDOR();
        void InsertExpiredAccounts();

        //Report Methods
        List<ToTransunion> GetTransunionList();
        List<ToPIF> GetPIF();
        List<ToBadDebt> GetBadDebts();
        List<ToCharity> GetCharities();
        List<ToCancel> GetCancels();
        List<ToInventory> GetInventories();
    }
}
