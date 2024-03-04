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

        public Task<List<RemoteConnection>> GetAllRemoteConnectionsAsync()
        {
            return _db.RemoteConnections.ToListAsync();
        }

        public Task<RemoteConnection> GetRemoteConnectionByIdAsync(long? id)
        {
            return Task.FromResult(_db.RemoteConnections.Find(id));
        }

        #endregion
        #region Settings
        public Task<int> AddSettingsAsync(Setting setting)
        {
            _db.Settings.Add(setting);
            return _db.SaveChangesAsync();
        }

        public Task<List<Setting>> GetAllSettingsAsync()
        {
            return _db.Settings.ToListAsync();
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

        public Task<DropFormat> GetDropFormatByIdAsync(long? id)
        {
            return Task.FromResult(_db.DropFormats.Find(id));
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

        #endregion
        #region Generic_Update_Delete
        public Task<int> DeleteObjectAsync(object obj)
        {
            var entity = _db.Entry(obj);
            entity.State = EntityState.Deleted;
            return _db.SaveChangesAsync();
        }
        public Task<int> UpdateObjectAsync(object obj)
        {
            var entity = _db.Entry(obj);
            entity.State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }
        #endregion
        #region FileFormats
        public Task<List<FileFormat>> GetAllFileFormatsAsync()
        {
            return _db.FileFormats.ToListAsync();
        }

        public Task<int> AddFileFormatAsync(FileFormat format)
        {
            _db.FileFormats.Add(format);
            return _db.SaveChangesAsync();
        }
        public Task<FileFormat> GetFileFormatByIdAsync(long? id)
        {
            return Task.FromResult(_db.FileFormats.Find(id));
        }
        #endregion
        #region FileFormatDetails
        public Task<List<FileFormatDetail>> GetAllFileFormatDetailsByFileFormatIdAsync(long id)
        {
            return _db.FileFormatDetails.Where(d => d.FileFormatId == id).ToListAsync();
        }

        public Task<int> AddFileFormatDetailAsync(FileFormatDetail detail)
        {
            _db.FileFormatDetails.Add(detail);
            return _db.SaveChangesAsync();
        }




        #endregion
    }
}
