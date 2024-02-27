using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Context;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{

    public class BeyondRepository : IBeyondRepository
    {
        private BeyondContext _db = new BeyondContext();

        public Task<int> AddSettingsAsync(Setting setting)
        {
            setting.Id = 0;
            var entity = _db.Entry(setting);
            entity.State = EntityState.Added;
            return _db.SaveChangesAsync();
        }

        public Task<int> DeleteSettingsAsync(Setting setting)
        {
            var entity = _db.Entry(setting);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }

        public Task<List<Setting>> GetAllSettingsAsync()
        {
            return _db.Settings.ToListAsync();
        }

        public Task<int> UpdateSettingsAsync(Setting setting)
        {
            var entity = _db.Entry(setting);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }
    }
}
