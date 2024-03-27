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
        public Task<List<string>> GetColumnTypesAsync()
        {
            return Task.FromResult(new List<string>
            {
                "double",
                "string",
                "DateTime",
                "long",
                "int",
                "StateZip"
            });
        }

        public Task<List<string>> GetDebtorPropertiesAsync()
        {
            List<string> fields = new List<string>();
            foreach (var field in typeof(Debtor).GetProperties())
            {
                if(field.GetMethod != null)
                    fields.Add(field.Name);
            }

            return Task.FromResult(fields);
        }

        public Task<List<string>> SetDebtorPropertiesAsync()
        {
            List<string> fields = new List<string>();
            foreach (var field in typeof(Debtor).GetProperties())
            {
                if (field.SetMethod != null)
                    fields.Add(field.Name);
            }

            return Task.FromResult(fields);
        }
    }
}
