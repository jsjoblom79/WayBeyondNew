using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class Debtor
    {
        public string? ClientDebtorNumber { get; set; }
        public DateTime? DateOfService { get; set; }
        public double AmountReferred { get; set; }
        public string? MedicalRecordNumber { get; set; }
        public string? BillingNumber { get; set; }
        public string? PatientsName { get; set; }
        public DateTime? PatientsDOB { get; set; }
        public string? PatientsSSN { get; set; }
        public string? PatientsEmployer { get; set; }
        public string? PatientsPhone { get; set; }
        public string? PatientsRelationship { get; set; }
        public string? PatientMiscData1 { get; set; }
        public string? PatientMiscData2 { get; set; }
        public string? PatientMiscData3 { get; set; }
        public string? PatientMiscData4 { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? InsuranceName { get; set; }
        public string? InsuranceAddress { get; set; }
        public string? InsuranceCity { get; set; }
        public string? InsuranceStateZip { get; set; }
        public string? InsuranceContact { get; set; }
        public string? InsurancePhone { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public string? DebtorLastName { get; set; }
        public string? DebtorFirstMiddleName { get; set; }
        public string? DebtorAddress1 { get; set; }
        public string? DebtorAddress2 { get; set; }
        public string? DebtorCity { get; set; }
        public string? DebtorStateZip { get; set; }
        public string? DebtorSpouse { get; set; }
        public string? DebtorPhone { get; set; }
        public string? DebtorCell { get; set; }
        public string? DebtorSSN { get; set; }
        public string? DebtorEmployerName { get; set; }
        public string? DebtorEmpPhone { get; set; }
        public string? DebtorEmpAddress { get; set; }
        public string? DebtorEmpCityState { get; set; }
        public string? SpouseEmployerName { get; set; }
        public string? SpouseEmpPhone { get; set; }
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
        public string? ComakerPhone { get; set; }
        public string? ComakerEmployer { get; set; }
        public string? ComakerSSN { get; set; }
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
    }
}
