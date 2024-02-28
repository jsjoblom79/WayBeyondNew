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

        //RemoteConnections
        Task<List<RemoteConnection>> GetAllRemoteConnectionsAsync();
        Task<int> UpdateRemoteConnectionsAsync(RemoteConnection connection);
        Task<int> AddRemoteConnectionAsync(RemoteConnection connection);
        Task<int> DeleteRemoteConnectionAsync(RemoteConnection connection);

        //Client
        Task<List<Client>> GetAllClientsAsync();
        Task<int> AddClientAsync(Client client);
        Task<int> UpdateClientAsync(Client client);
        Task<int> DeleteClientAsync(Client client);

        //FileLocations
        Task<List<FileLocation>> GetAllFileLocationsAsync();
        Task<int> AddFileLocationsAsync(FileLocation location);
        Task<int> UpdateFileLocationsAsync(FileLocation location);
        Task<int> DeleteFileLocationsAsync(FileLocation location);


    }
}
