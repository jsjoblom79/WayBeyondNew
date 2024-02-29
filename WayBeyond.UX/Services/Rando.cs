using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class Rando : IRando
    {
        public Task<List<string>> GetDebtorProperties()
        {
            List<string> fields = new List<string>();
            foreach (var field in typeof(Debtor).GetProperties())
            {
                fields.Add(field.Name);
            }

            return Task.FromResult(fields);
        }
    }
}
