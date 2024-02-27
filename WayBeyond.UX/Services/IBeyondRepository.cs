using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public interface IBeyondRepository
    {
        //Settings
        Task<List<Setting>> GetAllSettingsAsync();
        Task<int> UpdateSettingsAsync(Setting setting);
        Task<int> AddSettingsAsync(Setting setting);
        Task<int> DeleteSettingsAsync(Setting setting);
    }
}
