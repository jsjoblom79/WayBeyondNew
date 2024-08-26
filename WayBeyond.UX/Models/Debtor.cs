using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Navigation;

namespace WayBeyond.Data.Models
{
    public class Debtor
    {
        private string? _clientDebtorNumber;
        public string? ClientDebtorNumber
        {
            get
            {
                if (_clientDebtorNumber == null)
                {
                    if (MedicalRecordNumber != null && InvoiceNumber != null)
                    {
                        return $"{MedicalRecordNumber}/{InvoiceNumber}";
                    }
                    else { return string.Empty; }
                }
                else { return _clientDebtorNumber; }
            }
            set { _clientDebtorNumber = value;}
        }
        public DateTime? DateOfService { get; set; }
        public double AmountReferred { get; set; }
        public string? MedicalRecordNumber { get; set; }
        public string? BillingNumber { get; set; }
        public string? TransactionNumber { get; set; }  
        public string? PatientsName { get => !string.IsNullOrWhiteSpace(PatientsFirstName) ? $"{PatientsLastName}, {PatientsFirstName}" : $"{LastName}, {FirstMiddleName}"; }
        public string? PatientsFirstName { get; set; }
        public string? PatientsLastName { get; set; }
        private DateTime? _patientsDOB;
        public DateTime? PatientsDOB { get => _patientsDOB != null ? _patientsDOB : _debtorDOB; set { _patientsDOB = value; } }
        private string? _patientsSSN;
        public string? PatientsSSN { get => !string.IsNullOrWhiteSpace(_patientsSSN) ? _patientsSSN : _debtorSSN; set => _patientsSSN = value; }
        public string? PatientsEmployer { get; set; }
        private string? _patientsPhone;
        public string? PatientsPhone { get { if (!string.IsNullOrWhiteSpace(_patientsPhone)) { return _patientsPhone.Replace(" ", ""); } else { return null; } } set => _patientsPhone = value; }
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
        public string? InsurancePhone { get { if (!string.IsNullOrWhiteSpace(_insurancePhone)) { return _insurancePhone.Replace(" ", ""); } else { return null; } } set => _insurancePhone = value; }
        public string? InsurancePolicyNumber { get; set; }
        private string? _debtorLastName;
        public string? DebtorLastName
        {
            get
            {
                if(string.IsNullOrEmpty(_debtorLastName) && !string.IsNullOrEmpty(PatientsLastName))
                {
                    return PatientsLastName;
                }
                else
                {
                    return _debtorLastName;
                }
            }
            set
            {
                _debtorLastName = value;
            }
        }
        private string? _debtorFirstMiddleName;
        public string? DebtorFirstMiddleName
        {
            get
            {
                if(string.IsNullOrEmpty(_debtorFirstMiddleName) && !string.IsNullOrEmpty(PatientsFirstName))
                {
                    return PatientsFirstName;
                } else { return _debtorFirstMiddleName; }
            }
            set
            {
                _debtorFirstMiddleName = value;
            }
        }
        public string? DebtorAddress1 { get; set; }
        public string? DebtorAddress2 { get; set; }
        public string? DebtorCity { get; set; }
        public string? DebtorState { get; set; }
        public string? DebtorZip { get; set; }
        public string? DebtorSpouse { get; set; }
        private string? _debtorPhone;
        public string? DebtorPhone { get => _debtorPhone != null ? _debtorPhone.Replace(" ","") : null; set => _debtorPhone = value; }
        private string? _debtorCell;
        public string? DebtorCell { get => _debtorCell != null ? _debtorCell.Replace(" ","") : null; set => _debtorCell = value; }
        private string? _debtorSSN;
        public string? DebtorSSN { get =>
                !string.IsNullOrWhiteSpace(_debtorSSN) ? _debtorSSN : PatientsSSN;
                 set => _debtorSSN = value; }
        public string? DebtorEmployerName { get; set; }
        private string? _debtorEmpPhone;
        public string? DebtorEmpPhone { get { if (!string.IsNullOrWhiteSpace(_debtorEmpPhone)) { return _debtorEmpPhone.Replace(" ", ""); } else { return null; } } set => _debtorEmpPhone = value; }
        public string? DebtorEmpAddress { get; set; }
        public string? DebtorEmpCityState { get; set; }
        public string? SpouseEmployerName { get; set; }
        private string? _spouseEmpPhone;
        public string? SpouseEmpPhone { get => _spouseEmpPhone != null ? _spouseEmpPhone.Replace(" ", "") : null; set => _spouseEmpPhone = value; }
        public string? SpouseSSN { get; set; }
        public DateTime? SpouseDOB { get; set; }
        private DateTime? _debtorDOB;
        public DateTime? DebtorDOB
        {
            get
            {
                if(_debtorDOB == null)
                {
                    if(_patientsDOB != null)
                    {
                        return _patientsDOB;
                    } else { return null; }
                } else { return _debtorDOB; }
            }
            set { _debtorDOB = value; }
        }
        public string? DebtorEmail { get; set; }
        public string? DebtorLicenseNumber { get; set; }
        public string? PatientAddress { get; set; }
        public string? PatientCity { get; set; }
        public string? PatientState { get; set; }
        private string? _patientCityState;
        public string? PatientCityState { get => $"{PatientCity} {PatientState}"; set =>  _patientCityState = value;  }
        public string? AccountFor { get; set; }
        public string? ComakerLastName { get; set; }
        public string? ComakerFirstName { get; set; }
        public string? ComakerAddress1 { get; set; }
        public string? ComakerAddress2 { get; set; }
        public string? ComakerCity { get; set; }
        public string? ComakerStZip { get; set; }
        private string? _comakerPhone;
        public string? ComakerPhone { get => _comakerPhone != null ? _comakerPhone.Replace(" ", "") : null; set => _comakerPhone = value; }
        public string? ComakerEmployer { get; set; }
        private string? _comakerSSN;
        public string? ComakerSSN { get => _comakerSSN != null ? _comakerSSN.Replace("-", "") : null; set => _comakerSSN = value; }
        public DateTime? ComakerDOB { get; set; }
        public string? Notes { get; set; }
        public double PatientPaid { get; set; }
        public DateTime? PatientPaidDate { get; set; }
        public DateTime? DateLastPay { get; set; }
        public string? Skip { get; set; }

        public ClientId? ClientName { get; set; }
        public string? DebtorHomePhone
        {
            get => !string.IsNullOrWhiteSpace(_debtorCell) ? _debtorPhone : _debtorPhone;
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
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(DebtorState) || !string.IsNullOrEmpty(DebtorZip) && DebtorZip.Length >= 5)
                {
                    result = $"{DebtorState.Replace(" ","").Trim()} {DebtorZip.Substring(0, 5).Replace(" ","").Trim()}";
                }
                else
                {
                    if(!string.IsNullOrEmpty(DebtorZip) || !string.IsNullOrEmpty(DebtorState))
                    {
                        result = $"{DebtorState.Trim()} {DebtorZip.Trim()}";
                    }
                    else
                    {
                        result = $"{DebtorState} {DebtorZip}";
                    }
                }

                return result;
            }
        }
        public string? Location { get; set; }
        public string? PatientEmpPhone { get; set; }
        public string? RGHRecordStatus { get; set; }
        public bool? IsMedicare { get; set; } = false;

        public Client? Client { get; set; }// This is used for Epic Meditech Clients.

        public virtual double GetAmountReferred() => AmountReferred;

        public virtual DateTime? GetDateOfService() => DateOfService;
        public virtual string? GetPatientMiscData1() => PatientMiscData1;
        public virtual string? GetPatientMiscData2() => PatientMiscData2;
        public virtual string? GetInsuranceName() => InsuranceName;
    }
}
