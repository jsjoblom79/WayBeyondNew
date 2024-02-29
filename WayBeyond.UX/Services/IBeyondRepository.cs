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
        Task<int> AddSettingsAsync(Setting setting);


        //RemoteConnections
        Task<List<RemoteConnection>> GetAllRemoteConnectionsAsync();
        Task<int> AddRemoteConnectionAsync(RemoteConnection connection);

        //Client
        Task<List<Client>> GetAllClientsAsync();
        Task<int> AddClientAsync(Client client);
        Task<List<Client>> GetClientByDropFormatIdAsync(long id);

        //FileLocations
        Task<List<FileLocation>> GetAllFileLocationsAsync();
        Task<int> AddFileLocationsAsync(FileLocation location);

        //DropFormats
        Task<List<DropFormat>> GetAllDropFormatsAsync();
        Task<int> AddDropFormatAsync(DropFormat dropFormat);


        //DropFormatDetails
        Task<List<DropFormatDetail>> GetAllDropFormatDetailsByDropFormatId(long id);
        Task<int> AddDropFormatDetailAsync(DropFormatDetail detail);

        //Generic Delete and Update
        Task<int> DeleteObjectAsync(object obj);
        Task<int> UpdateObjectAsync(object obj);
    }
}
