using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class Patient
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? AddressLine1 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zip { get; set; }

        public DateTime? Dob { get; set; }

        public string? Ssn { get; set; }

        public string? Mrn { get; set; }
    }
}
