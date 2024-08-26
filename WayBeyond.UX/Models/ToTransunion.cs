using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class ToTransunion
    {
        public string? RegistrationFsc1 { get; set; }
        public string? RegistrationFsc2 { get; set; }
        public string? PLST { get; set; }
        public string? PFIRST { get; set; }
        public string? MRN { get; set; }
        public string? OURAcct { get; set; }
        public string? PDOB { get; set; }
        public string? PSSN { get; set; }
        public string? PAddress { get; set; }
        public string? PCity { get; set; }
        public string? PState { get; set; }
        public string? PZip { get; set; }
        public string? Telephone { get; set; }
        public string? INV { get; set; }
        public string? Balance { get; set; }
        public string? ServiceDate { get; set; }
        public string? Employer { get; set; }
        public string? GLST { get; set; }
        public string? GFIRST { get; set; }
        public string? Gssn { get; set; }
        public string? GDOB { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? MessageTelephone { get; set; }

        private DateTime? PatientDOB
        {
            get
            {
                if (PDOB == null)
                {
                    return null;
                }
                DateTime.TryParse(PDOB, out DateTime result);
                return result;
            }
        }
        private DateTime? GuarDOB
        {
            get
            {
                if (GDOB == null) { return null; }
                DateTime.TryParse(GDOB, out DateTime result);
                return result;
            }
        }
        private double Amount
        {
            get
            {
                double.TryParse(Balance, out double result);
                return result;
            }
        }
        private DateTime DateOfService
        {
            get
            {

                DateTime.TryParse(ServiceDate, out DateTime result);
                return result;
            }
        }
        public override string ToString()
        {
            //possible that the date's will need to be formatted to M/d/yyyy 
            return $"{RegistrationFsc1},{RegistrationFsc2},{PLST},{PFIRST},{MRN},{OURAcct.Substring(0, OURAcct.Length - 3)},{PatientDOB:MM/dd/yyyy},{PSSN},{PAddress},{PCity}," +
                $"{PState},{PZip},{Telephone},{INV},{Amount:#.##}," +
                $"{DateOfService:MM/dd/yyyy},{Employer},{GLST},{GFIRST},{Gssn},{GuarDOB:MM/dd/yyyy},{Address},{City},{State},{Zip},{MessageTelephone}";
        }
    }
}
