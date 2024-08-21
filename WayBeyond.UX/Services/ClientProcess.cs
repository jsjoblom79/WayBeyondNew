using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using System.IO;
using System.Windows;
using Serilog;
using SQLitePCL;


namespace WayBeyond.UX.Services
{
    public class ClientProcess : IClientProcess
    {
        private ITransfer _transfer;
        private IBeyondRepository _db;
        private DropFileWrite _dropFileWrite;
        public ClientProcess(ITransfer transfer, IBeyondRepository db)
        {
            _transfer = transfer;   
            _db = db;
            _dropFileWrite = new(db); //, transfer);
        }

        public event Action<string> ProcessUpdates = delegate { };

        public async Task<bool> ProcessClientFile(FileObject file, Client client)
        {
            //determine if the batch is available or create one if needed.
            var batch = await GetBatchFileAsync();


            FileObject downloadedFile = file;
            //if file is remote download a copy of it.
            if (file.FileType == FileType.REMOTE)
            {
                downloadedFile = await _transfer.DownloadFileAsync(file);
            }
            //instantiate the excel service.
            ExcelService excelService = new ExcelService();
            excelService.Update += ProcessUpdates;

            List<Debtor> debtors;
            //Read debtor File create list of debtors
            try
            {
                debtors = await excelService.ReadClientFile(client, downloadedFile);
            }
            catch (NullReferenceException ex)
            {
                return false;
            }
            

            //WriteDrop file
            if(await WriteDropFileAsync(client, debtors , batch))
            {
                //if drop file created then write Client Loads
                await CreateClientLoadAsync(client, debtors, batch, downloadedFile);
            } else
            {
                MessageBox.Show("An error occured Writing to the drop file. Please see Log file for detailed information","Drop File Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            await Task.Run(()=>ProcessUpdates($"Drop File for client: {client.ClientName} has been created."));

            //Archive remote or local Files.
            if (file.RemoteConnection != null)
            {
                if( await _transfer.ArchiveFileAsync(file))
                    await Task.Run(()=>ProcessUpdates($"Remote file: {file.FileName} has been archived."));
                if (await _transfer.ArchiveFileAsync(downloadedFile))
                    await Task.Run(()=>ProcessUpdates($"Local File: {downloadedFile.FileName} has been archived."));
            }
            else
            {
                if (await _transfer.ArchiveFileAsync(downloadedFile))
                    await Task.Run(()=>ProcessUpdates($"Local File: {downloadedFile.FileName} has been archived."));
            }
            
            

            return true;
        }

        public Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
           return Task.FromResult(_dropFileWrite.WriteDropFile(client, debtors, batch));
        }

        public async Task<bool> CreateClientLoadAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            return await _dropFileWrite.CreateClientLoad(client, debtors, batch, file);
        }

        public async Task<ProcessedFileBatch> GetBatchFileAsync()
        {
            var batch = await _db.GetCurrentBatch();
            if (batch != null)
            {
                await Task.Run(()=>ProcessUpdates($"Batch: {batch.BatchName} has been created."));
            }
            
            return batch;
        }

        public Task<bool> ProcessEpicClientAsync(FileObject file, Client[]? client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProcessEpicClientAsync(FileObject file, Client? client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteDropFileAsync(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            throw new NotImplementedException();
        }
    }
}
