using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.Services
{
    public interface IRando
    {
        Task<List<string>> GetDebtorProperties();
    }
}
