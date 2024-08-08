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
        Task<RemoteConnection> GetRemoteConnectionByIdAsync(long? id);
        Task<RemoteConnection> GetRemoteConnectionByNameAsync(string name);

        //Client
        Task<List<Client>> GetAllClientsAsync();
        Task<int> AddClientAsync(Client client);
        Task<List<Client>> GetClientByDropFormatIdAsync(long id);
        Task<Client> GetClientByClientIdAsync(long id);
        Client GetClientByClientId(long id);
        Client GetClientByClientName(string name);

        //FileLocations
        Task<List<FileLocation>> GetAllFileLocationsAsync();
        Task<int> AddFileLocationsAsync(FileLocation location);
        Task<List<FileLocation>> GetFileLocationsByNameAsync(LocationName name);
        Task<FileLocation> GetFileLocationByNameAsync(LocationName name);
        Task<FileLocation> GetSingleFileLocationByNameAsync(LocationName name);
        //DropFormats
        Task<List<DropFormat>> GetAllDropFormatsAsync();
        Task<int> AddDropFormatAsync(DropFormat dropFormat);
        Task<DropFormat> GetDropFormatByIdAsync(long? id);

        //DropFormatDetails
        Task<List<DropFormatDetail>> GetAllDropFormatDetailsByDropFormatId(long? id);
        Task<int> AddDropFormatDetailAsync(DropFormatDetail detail);

        //FileFormats
        Task<List<FileFormat>> GetAllFileFormatsAsync();
        Task<int> AddFileFormatAsync(FileFormat format);
        Task<FileFormat> GetFileFormatByIdAsync(long? id);

        //FileFormatDetails
        Task<List<FileFormatDetail>> GetAllFileFormatDetailsByFileFormatIdAsync(long? id);
        Task<int> AddFileFormatDetailAsync(FileFormatDetail detail);

        //Settings
        ValueTask<Setting> GetSettingByKeyAsync(string key);

        //Generic Delete and Update
        Task<int> DeleteObjectAsync(object obj);
        Task<int> UpdateObjectAsync(object obj);

        //Processed File Batch
        Task<List<ProcessedFileBatch>> GetAllProcessedFilesBatchAsync();
        Task<ProcessedFileBatch> GetProcessedFilesBatchByIdAsync(long? id);
        Task<ProcessedFileBatch?> GetProcessedFilesBatchByDateAsync(DateTime? date);
        Task<List<DateTime?>> GetProcessedFilesBatchDatesAsync();
        Task<int> AddProcessedFilesBatch(ProcessedFileBatch batch);
        Task<ProcessedFileBatch> GetCurrentBatch();
        //ClientLoad
        Task<int> AddClientLoadAsync(ClientLoad clientLoad);
        Task<List<ClientLoad>> GetAllClientLoadsByBatchIdAsync(long? id);
        Task<List<ClientLoad>> GetClientLoadsByDateAsync(DateTime date);
    }
}
