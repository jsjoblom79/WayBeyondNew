using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace WayBeyond.Data.Models
{
    public class Debtor
    {
        public string? ClientDebtorNumber { get;  set; }
        public DateTime? DateOfService { get; set; }
        public double AmountReferred { get; set; }
        public string? MedicalRecordNumber { get; set; }
        public string? BillingNumber { get; set; }
        public string? PatientsName { get; set; }
        public string? PatientsFirstName { get; set; }
        public string? PatientsLastName { get; set; }
        public DateTime? PatientsDOB { get; set; }
        private string? _patientsSSN;
        public string? PatientsSSN { get { if (!string.IsNullOrWhiteSpace(_patientsSSN)) { return _patientsSSN.Replace("-", ""); } else { return null; } } set => _patientsSSN = value; }
        public string? PatientsEmployer { get; set; }
        private string? _patientsPhone;
        public string? PatientsPhone { get { if (!string.IsNullOrWhiteSpace(_patientsPhone)) { return _patientsPhone.Replace("-", ""); } else { return null; } } set => _patientsPhone = value; }
        public string? PatientsRelationship { get; set; }
        public string? PatientMiscData1 { get; set; }
        public string? PatientMiscData2 { get; set; }
        public string? PatientMiscData3 { get; set; }
        public string? PatientMiscData4 { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? InsuranceName { get; set; }
        public string? InsuranceAddress { get; set; }
        public string? InsuranceCity { get; set; }
        public string? InsuranceStateZip { get; set; }
        public string? InsuranceContact { get; set; }
        private string? _insurancePhone;
        public string? InsurancePhone { get { if (!string.IsNullOrWhiteSpace(_insurancePhone)) { return _insurancePhone.Replace("-", ""); } else { return null; } } set => _insurancePhone = value; }
        public string? InsurancePolicyNumber { get; set; }
        public string? DebtorLastName { get; set; }
        public string? DebtorFirstMiddleName { get; set; }
        public string? DebtorAddress1 { get; set; }
        public string? DebtorAddress2 { get; set; }
        public string? DebtorCity { get; set; }
        public string? DebtorState { get; set; }
        public string? DebtorZip { get; set; }
        public string? DebtorSpouse { get; set; }
        private string? _debtorPhone;
        public string? DebtorPhone { get => _debtorPhone.Replace("-", ""); private set => _debtorPhone = value; }
        private string? _debtorCell;
        public string? DebtorCell { get => _debtorCell.Replace("-", ""); set => _debtorCell = value; }
        private string? _debtorSSN;
        public string? DebtorSSN { get => _debtorSSN.Replace("-",""); set => _debtorSSN = value; }
        public string? DebtorEmployerName { get; set; }
        private string? _debtorEmpPhone;
        public string? DebtorEmpPhone { get { if (!string.IsNullOrWhiteSpace(_debtorEmpPhone)) { return _debtorEmpPhone.Replace("-", ""); } else { return null; } } set => _debtorEmpPhone = value; }
        public string? DebtorEmpAddress { get; set; }
        public string? DebtorEmpCityState { get; set; }
        public string? SpouseEmployerName { get; set; }
        private string? _spouseEmpPhone;
        public string? SpouseEmpPhone { get => _spouseEmpPhone.Replace("-", ""); set => _spouseEmpPhone = value; }
        public string? SpouseSSN { get; set; }
        public DateTime? SpouseDOB { get; set; }
        public DateTime? DebtorDOB { get; set; }
        public string? DebtorEmail { get; set; }
        public string? DebtorLicenseNumber { get; set; }
        public string? PatientAddress { get; set; }
        public string? PatientCityState { get; set; }
        public string? AccountFor { get; set; }
        public string? ComakerLastName { get; set; }
        public string? ComakerFirstName { get; set; }
        public string? ComakerAddress1 { get; set; }
        public string? ComakerAddress2 { get; set; }
        public string? ComakerCity { get; set; }
        public string? ComakerStZip { get; set; }
        private string? _comakerPhone;
        public string? ComakerPhone { get => _comakerPhone.Replace("-", ""); set => _comakerPhone = value; }
        public string? ComakerEmployer { get; set; }
        private string? _comakerSSN;
        public string? ComakerSSN { get => _comakerSSN.Replace("-", ""); set => _comakerSSN = value; }
        public DateTime? ComakerDOB { get; set; }
        public string? Notes { get; set; }
        public double PatientPaid { get; set; }
        public DateTime? PatientPaidDate { get; set; }
        public DateTime? DateLastPay { get; set; }
        public string? Skip { get; set; }

        public string? DebtorHomePhone
        {
            get
            {
                if(string.IsNullOrWhiteSpace(DebtorPhone) && !string.IsNullOrWhiteSpace(DebtorCell))
                {
                    return DebtorCell;
                }
                else
                {
                    if (!DebtorPhone.Equals(DebtorCell) && !string.IsNullOrWhiteSpace(DebtorCell))
                    {
                        return DebtorCell;
                    }
                    else
                    {
                        if (DebtorPhone.Equals(DebtorCell))
                        {
                            return DebtorCell;
                        }
                        else
                        {
                            return DebtorPhone;
                        }
                    }
                }
            }
        }
        public string? FirstMiddleName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DebtorFirstMiddleName))
                {
                    return PatientsFirstName;
                }
                else
                {
                    return DebtorFirstMiddleName;
                }
            }
        }
        public string? LastName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DebtorLastName))
                {
                    return PatientsLastName;
                }
                else
                {
                    return DebtorLastName;
                }
            }
        }
        public string? DebtorStateZip
        {
            get { return $"{DebtorState} {DebtorZip}"; }
        }

        public string? GetClientDebtorNumber()
        {
            if (ClientDebtorNumber == null)
            {
                return $"{MedicalRecordNumber}/{InvoiceNumber}";
            }
            else
            {
                return ClientDebtorNumber;
            }
        } 
    }
}
