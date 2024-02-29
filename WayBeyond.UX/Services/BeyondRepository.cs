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

        #region RemoteConnections
        public Task<int> AddRemoteConnectionAsync(RemoteConnection connection)
        {
            _db.RemoteConnections.Add(connection);
            return _db.SaveChangesAsync();
        }

        public Task<int> UpdateRemoteConnectionsAsync(RemoteConnection connection)
        {
            var entity = _db.Entry(connection);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }
        public Task<int> DeleteRemoteConnectionAsync(RemoteConnection connection)
        {
            var entity = _db.Entry(connection);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }
        public Task<List<RemoteConnection>> GetAllRemoteConnectionsAsync()
        {
            return _db.RemoteConnections.ToListAsync();
        }


        #endregion
        #region Settings
        public Task<int> AddSettingsAsync(Setting setting)
        {
            _db.Settings.Add(setting);
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


        #endregion
        #region Clients
        public Task<List<Client>> GetAllClientsAsync()
        {
            return _db.Clients.ToListAsync();
        }

        public Task<int> AddClientAsync(Client client)
        {
            try
            {
                _db.Clients.Add(client);
                return _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public Task<int> UpdateClientAsync(Client client)
        {
            var entity = _db.Entry(client);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        public Task<int> DeleteClientAsync(Client client)
        {
            var entity = _db.Entry(client);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }


        public Task<List<Client>> GetClientByDropFormatIdAsync(long id)
        {
            return _db.Clients.Where(c => c.DropFormatId == id).ToListAsync();
        }
        #endregion
        #region FileLocations
        public Task<List<FileLocation>> GetAllFileLocationsAsync()
        {
            return _db.FileLocations.ToListAsync();
        }

        public Task<int> AddFileLocationsAsync(FileLocation location)
        {
            _db.FileLocations.Add(location);
            return _db.SaveChangesAsync();
        }

        public Task<int> UpdateFileLocationsAsync(FileLocation location)
        {
            var entity = _db.Entry(location);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        public Task<int> DeleteFileLocationsAsync(FileLocation location)
        {
            var entity = _db.Entry(location);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }
        #endregion
        #region DropFormats
        public Task<List<DropFormat>> GetAllDropFormatsAsync()
        {
            return _db.DropFormats.ToListAsync();
        }

        public Task<int> AddDropFormatAsync(DropFormat dropFormat)
        {
            _db.DropFormats.Add(dropFormat);
            return _db.SaveChangesAsync();
        }

        public Task<int> UpdateDropFromatAsync(DropFormat dropFormat)
        {
            var entity = _db.Entry(dropFormat);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        public Task<int> DeleteDropFromatAsync(DropFormat dropFormat)
        {
            var entity = _db.Entry(dropFormat);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }
        #endregion

        #region DropFormatDetails
        public Task<List<DropFormatDetail>> GetAllDropFormatDetailsByDropFormatId(long id)
        {
            return _db.DropFormatDetails.Where(d => d.DropFormatId == id).ToListAsync();
        }

        public Task<int> AddDropFormatDetailAsync(DropFormatDetail detail)
        {
            _db.DropFormatDetails.Add(detail);
            return _db.SaveChangesAsync();
        }

        public Task<int> UpdateDropFormatDetailAsync(DropFormatDetail detail)
        {
            var entity = _db.Entry(detail);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        public Task<int> DeleteDropFormatDetailAsync(DropFormatDetail detail)
        {
            var entity = _db.Entry(detail);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }

        public Task<int> DeleteObject(object obj)
        {
            var entity = _db.Entry(obj);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }
        #endregion
    }
}
