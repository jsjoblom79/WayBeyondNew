using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public interface IRando
    {
        Task<List<string>> GetDebtorPropertiesAsync();
        Task<List<string>> GetColumnTypesAsync();
    }
}
