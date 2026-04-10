using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class BadEmailAddresses
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Domain { get; set; }
        public string? EmailAddress { get; set; }
        public BadEmailType BadEmailType { get; set; }
        override public string ToString()
        {
            return $"Username: {Username}, Domain: {Domain}, EmailAddress: {EmailAddress}, BadEmailType: {BadEmailType}";
        }
    }
}
