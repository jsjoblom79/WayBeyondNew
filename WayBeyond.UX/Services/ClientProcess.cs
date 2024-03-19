using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using System.IO;
using System.Windows;
using Serilog;


namespace WayBeyond.UX.Services
{
    public class ClientProcess : IClientProcess
    {
        private ITransfer _transfer;
        private IBeyondRepository _db;
        public ClientProcess(ITransfer transfer, IBeyondRepository db)
        {
            _transfer = transfer;   
            _db = db;
        }

        public event Action<string> ProcessUpdates = delegate { };

        public async Task<bool> ProcessClientFile(FileObject file, Client client)
        {
            //determine if the batch is available or create one if needed.
            var batch = await GetBatchFile();


            FileObject downloadedFile = file;
            //if file is remote download a copy of it.
            if (file.FileType == FileType.REMOTE)
            {
                downloadedFile = await _transfer.DownloadFileAsync(file);
            }
            //instantiate the excel service.
            ExcelService excelService = new ExcelService();
            excelService.Update += ProcessUpdates;

            //Read debtor File create list of debtors.
            var debtors = await excelService.ReadClientFile(client, downloadedFile);

            //WriteDrop file
            if(await WriteDropFile(client, debtors , batch))
            {
                //if drop file created then write Client Loads
                await CreateClientLoad(client, debtors, batch, downloadedFile);
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

        private Task<bool> WriteDropFile(Client client, List<Debtor> debtors, ProcessedFileBatch batch)
        {
            var drop = client.DropFormat;
            var dropDetails = drop.DropFormatDetails;
            var stringBuilder = new StringBuilder();

            try
            {
                //Writes the header record for the Drop File.
                foreach (var detail in dropDetails)
                {
                    stringBuilder.Append(detail.Field);
                    stringBuilder.Append('\t');
                }
                stringBuilder.AppendLine();
                foreach (var debtor in debtors)
                {
                    foreach (var detail in dropDetails)
                    {
                        switch (detail.FieldType)
                        {
                            case "DATE":
                                if (debtor.GetType().GetProperty(detail.Field).GetValue(debtor) != null)
                                    stringBuilder.Append($"{((DateTime)debtor.GetType().GetProperty(detail.Field).GetValue(debtor)):MM/dd/yy}");
                                break;
                            case "CURRENCY":
                                if (debtor.GetType().GetProperty(detail.Field).GetValue(debtor) != null)
                                    stringBuilder.Append($"{((double)debtor.GetType().GetProperty(detail.Field).GetValue(debtor)):####.00}");
                                break;
                            default:
                                stringBuilder.Append(debtor.GetType().GetProperty(detail.Field).GetValue(debtor));
                                break;
                        }
                        //stringBuilder.Append(debtor.GetType().GetProperty(detail.Field).GetValue(debtor));
                        stringBuilder.Append('\t');
                    }
                    stringBuilder.AppendLine();
                }
                var path = _db.GetFileLocationByNameAsync(LocationName.Prepared);
                System.IO.File.WriteAllText($@"{path.Result[0].Path}{client.ClientId}_{DateTime.Now:yyyyMMdd-HHmmss}_{client.DropFileName}", stringBuilder.ToString());
                if (System.IO.File.Exists($@"{path.Result[0].Path}{client.ClientId}_{DateTime.Now:yyyyMMdd-HHmmss}_{client.DropFileName}"))
                {
                    return Task.FromResult(true);
                }else
                {
                    return Task.FromResult(false);
                }
            } catch (Exception ex)
            {
                Log.Error(ex.StackTrace, ex);
                return Task.FromResult(false);
            }
            
            
        }

        private async Task<bool> CreateClientLoad(Client client, List<Debtor> debtors, ProcessedFileBatch batch, FileObject file)
        {
            var load = new ClientLoad
            {
                ClientId = client.Id,
                ClientName = client.ClientName,
                Balance = debtors.Sum(d => d.AmountReferred),
                DebtorCount = debtors.Count(),
                CreateDate = DateTime.Now,
                FileName = file.FileName,
                DateOnLoadFile = file.CreateDate,
                DropNumber = client.DropNumber,
                ProcessedFileBatchId = batch.Id
            };

            if(await _db.AddClientLoadAsync(load) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private async Task<ProcessedFileBatch> GetBatchFile()
        {
            var batch = await _db.GetProcessedFilesBatchByDateAsync(DateTime.Now);

            if (batch == null)
            {
                batch = new ProcessedFileBatch
                {
                    BatchName = $"Load Files - {DateTime.Now.Date}",
                    CreateDate = DateTime.Now.Date,
                    CreatedBy = Environment.UserName

                };
                if (await _db.AddProcessedFilesBatch(batch) > 0)
                {
                    await Task.Run(()=>ProcessUpdates($"Batch: {batch.BatchName} has been created."));
                }
            }

            return batch;
        }
        
    }
}
