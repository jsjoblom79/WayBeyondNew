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
        #endregion
    }
}
