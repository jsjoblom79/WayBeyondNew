using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class Duplicate
    {
        public int Id { get; set; }
        public string? DebtorName { get; set; }
        public string? ClientDebtorNumber { get; set; }
        public string? Xfor { get; set; }
        public int ClientNumber { get; set; }
        public double AmountRefered { get; set; }
        public int LoadLine { get; set; }
        public DateTime? AddDate { get; set; } = DateTime.Now;
    }
}
