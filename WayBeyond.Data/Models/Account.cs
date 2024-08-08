using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class Account
    {
        public string ClientDebtorNumber { get; set; } = null!;

        public string? RegistrationFsc1 { get; set; }

        public string? RegistrationFsc2 { get; set; }

        public string? AccountNumber { get; set; }

        public double? Balance { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? DateLastPay { get; set; }

        public string? StatusCode { get; set; }

        public DateTime? DateOfReferral { get; set; }

        public double? AmountReferred { get; set; }

        public DateTime? CloseDate { get; set; }

        public string? MRN { get; set; }

        public string? Inv { get; set; }

        public string? Scode { get; set; }

        public bool? Active { get; set; }
    }
}
