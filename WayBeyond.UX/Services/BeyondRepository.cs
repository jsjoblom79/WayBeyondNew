using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Context;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{

    public class BeyondRepository : IBeyondRepository
    {
        private BeyondContext _db = new BeyondContext(ConfigurationManager.ConnectionStrings["WayBeyond"].ConnectionString);

        #region RemoteConnections
        public Task<int> AddRemoteConnectionAsync(RemoteConnection connection)
        {
            
            _db.RemoteConnections.Add(connection);
            Log.Information($"[ADD] RemoteConnection: {connection.Name}");
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
        public Task<RemoteConnection> GetRemoteConnectionByNameAsync(string name)
        {
            return _db.RemoteConnections.Where(c => c.Name == name).FirstOrDefaultAsync();
        }

        #endregion
        #region Settings
        public Task<int> AddSettingsAsync(Setting setting)
        {
            _db.Settings.Add(setting);
            Log.Information($"[ADD] Setting: {setting.Key} Value: {setting.Value}");
            return _db.SaveChangesAsync();
        }

        public Task<List<Setting>> GetAllSettingsAsync()
        {
            return _db.Settings.ToListAsync();
        }
        public ValueTask<Setting> GetSettingByKeyAsync(string key)
        {
            return _db.Settings.FindAsync(key);
        }
        #endregion
        #region Clients
        public async Task<List<Client>> GetAllClientsAsync()
        {
            var clients = _db.Clients;
            foreach (var client in clients)
            {
                if(client.DropFormatId != null)
                    client.DropFormat = await GetDropFormatByIdAsync(client.DropFormatId);
                if(client.FileFormatId != null)
                    client.FileFormat = await GetFileFormatByIdAsync(client.FileFormatId);
            }
            return clients.ToList();
        }

        public Task<int> AddClientAsync(Client client)
        {
            try
            {
                _db.Clients.Add(client);
                Log.Information($"[ADD] Client:{client.ClientName}");
                return _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.StackTrace}", ex);
                return Task.FromResult(0);
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
            Log.Information($"[ADD] FileLocation:{location.FileLocationName}");
            return _db.SaveChangesAsync();
        }

        public Task<List<FileLocation>> GetFileLocationsByNameAsync(LocationName name)
        {
            var locations = _db.FileLocations.Where(l => l.FileLocationName == name.ToString());
            foreach (var location in locations)
            {
                if (location.FileType == FileType.REMOTE)
                {
                    location.RemoteConnection = _db.RemoteConnections.Find(location.RemoteConnectionId);
                }
            }
            return locations.ToListAsync();
        }
        public async Task<FileLocation> GetFileLocationByNameAsync(LocationName name)
        {
            var location = _db.FileLocations.Where(l => l.FileLocationName == name.ToString()).FirstOrDefault();

            if (location != null && location.FileType == FileType.REMOTE)
            {
                location.RemoteConnection = _db.RemoteConnections.Find(location.RemoteConnectionId);
            }
            
            return location;
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
            Log.Information($"[ADD] DropFormat: {dropFormat.DropName}");
            return _db.SaveChangesAsync();
        }

        public async Task<DropFormat> GetDropFormatByIdAsync(long? id)
        {
            var format = _db.DropFormats.Find(id);
            format.DropFormatDetails = await GetAllDropFormatDetailsByDropFormatId(id);
            return format;
        }
        #endregion
        #region DropFormatDetails
        public Task<List<DropFormatDetail>> GetAllDropFormatDetailsByDropFormatId(long? id)
        {
            return _db.DropFormatDetails.Where(d => d.DropFormatId == id).ToListAsync();
        }

        public Task<int> AddDropFormatDetailAsync(DropFormatDetail detail)
        {
            _db.DropFormatDetails.Add(detail);
            Log.Information($"[ADD] DropFormatDetail: {detail.Field} DropFormatId: {detail.DropFormatId}");
            return _db.SaveChangesAsync();
        }

        #endregion
        #region Generic_Update_Delete
        public Task<int> DeleteObjectAsync(object obj)
        {
            var entity = _db.Entry(obj);
            entity.State = EntityState.Deleted;

            Log.Information($"[DEL] record: {obj.GetType().Name}");
            return _db.SaveChangesAsync();
        }
        public Task<int> UpdateObjectAsync(object obj)
        {
            var entity = _db.Entry(obj);
            entity.State = EntityState.Modified;
            Log.Information($"[UPD] record: {obj.GetType().Name}");
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
            Log.Information($"[ADD] FileFormat: {format.FileFormatName}");
            return _db.SaveChangesAsync();
        }
        public async Task<FileFormat> GetFileFormatByIdAsync(long? id)
        {
            var format = _db.FileFormats.Find(id);
            format.FileFormatDetails = await GetAllFileFormatDetailsByFileFormatIdAsync(id);
            return format;
        }
        #endregion
        #region FileFormatDetails
        public Task<List<FileFormatDetail>> GetAllFileFormatDetailsByFileFormatIdAsync(long? id)
        {
            return _db.FileFormatDetails.Where(d => d.FileFormatId == id).ToListAsync();
        }

        public Task<int> AddFileFormatDetailAsync(FileFormatDetail detail)
        {
            _db.FileFormatDetails.Add(detail);
            Log.Information($"[ADD] FileFormatDetail: {detail.Field} FormatId: {detail.FileFormatId}");
            return _db.SaveChangesAsync();
        }
        #endregion
        #region ProcessedFileBatch
        public Task<List<ProcessedFileBatch>> GetAllProcessedFilesBatchAsync()
        {

            return _db.ProcessedFileBatches.ToListAsync();
        }

        public Task<ProcessedFileBatch> GetProcessedFilesBatchByIdAsync(long? id)
        {
            return Task.FromResult(_db.ProcessedFileBatches.Find(id));
        }

        public Task<ProcessedFileBatch?> GetProcessedFilesBatchByDateAsync(DateTime? date)
        {
            return _db.ProcessedFileBatches.Where(b => b.CreateDate.Value.Date == date.Value.Date).FirstOrDefaultAsync();
        }
        
        public Task<List<DateTime?>> GetProcessedFilesBatchDatesAsync()
        {
            return _db.ProcessedFileBatches.Select(b => b.CreateDate).OrderBy(b => b.Value.Date).ToListAsync();
        }

        public Task<int> AddProcessedFilesBatch(ProcessedFileBatch batch)
        {
            _db.ProcessedFileBatches.Add(batch);
            return _db.SaveChangesAsync();
        }
        public async Task<ProcessedFileBatch> GetCurrentBatch()
        {
            var batch = await GetProcessedFilesBatchByDateAsync(DateTime.Now);

            if (batch == null)
            {
                batch = new ProcessedFileBatch
                {
                    BatchName = $"Load Files - {DateTime.Now.Date}",
                    CreateDate = DateTime.Now.Date,
                    CreatedBy = Environment.UserName

                };

                if (await AddProcessedFilesBatch(batch) > 0)
                {
                    return batch;
                }
            }
            return batch;
        }
        #endregion
        #region ClientLoads
        public Task<int> AddClientLoadAsync(ClientLoad clientLoad)
        {
            _db.ClientLoads.Add(clientLoad);
            return _db.SaveChangesAsync();
        }

        public Task<List<ClientLoad>> GetAllClientLoadsByBatchIdAsync(long? id) => _db.ClientLoads.Where(l => l.ProcessedFileBatchId == id).ToListAsync();

        public Task<List<ClientLoad>> GetClientLoadsByDateAsync(DateTime date) => _db.ClientLoads.Where(l => l.CreateDate.Value.Date == date.Date).ToListAsync();

        public async Task<Client> GetClientByClientIdAsync(long id)
        {
            var client = _db.Clients.Where(c => c.ClientId == id).FirstOrDefault();
            client.DropFormat = await GetDropFormatByIdAsync(client.DropFormatId);
            return client;
        }

        public Client? GetClientByClientId(long id)
        {
            return _db.Clients.Where(c => c.ClientId == id).FirstOrDefault();
        }

        public async Task<FileLocation> GetSingleFileLocationByNameAsync(LocationName name)
        {
            var location = await _db.FileLocations.Where(l => l.FileLocationName == name.ToString()).FirstOrDefaultAsync();
            if (location != null)
            {
                return location;
            }
            else return null;

        }

        public Client? GetClientByClientName(string name)
        {
           Client? client = _db.Clients.Where(c => c.ClientName == name).FirstOrDefault();
            return client;
        }




        #endregion
    }
}
