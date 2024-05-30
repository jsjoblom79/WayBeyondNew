using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public interface IEpicClientProcess
    {
        Task<bool> ProcessEpicClientAsync(FileObject file, Client client);
        Task<ProcessedFileBatch> GetBatchFileAsync();
        Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch);
        Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file);

        event Action<string> ProcessUpdates;

    }
}
