using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using System.IO;


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


            //TODO: Add functionality to create or capture new processedFiles Batch
            var batch = await _db.GetProcessedFilesBatchByDateAsync(DateTime.Now);

            if(batch == null)
            {
                batch = new ProcessedFileBatch
                {
                    BatchName = $"Load Files - {DateTime.Now.Date}",
                    CreateDate = DateTime.Now.Date,
                    CreatedBy = Environment.UserName

                };
                if(await _db.AddProcessedFilesBatch(batch) > 0)
                {
                    ProcessUpdates($"Batch: {batch.BatchName} has been created.");
                }
            }

            FileObject downloadedFile = file;
            if (file.FileType == FileType.REMOTE)
            {
                downloadedFile = await _transfer.DownloadFileAsync(file);
            }
            
            ExcelService excelService = new ExcelService();
            excelService.Update += ProcessUpdates;
            WriteDropFile(client, await excelService.ReadClientFile(client, downloadedFile));
            ProcessUpdates($"Drop File for client: {client.ClientName} has been created.");

            //Archive remote or local Files.
            if (file.RemoteConnection != null)
            {
                if( await _transfer.ArchiveFileAsync(file))
                    ProcessUpdates($"Local file: {file.FileName} has been archived.");
            }
            else
            {
                if (await _transfer.ArchiveFileAsync(downloadedFile))
                    ProcessUpdates($"Remote File: {downloadedFile.FileName} has been archived.");
            }
            
            

            return true;
        }

        private void WriteDropFile(Client client, List<Debtor> debtors)
        {
            var drop = client.DropFormat;
            var dropDetails = drop.DropFormatDetails;
            var stringBuilder = new StringBuilder();
            //Writes the header record for the Drop File.
            foreach (var detail in dropDetails)
            {
                stringBuilder.Append(detail.Field);
                stringBuilder.Append('\t');
            }
            stringBuilder.AppendLine();
            foreach (var debtor in debtors)
            {
                foreach(var detail in dropDetails)
                {
                    switch (detail.FieldType)
                    {
                        case "DATE":
                            if(debtor.GetType().GetProperty(detail.Field).GetValue(debtor) != null)
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
            var path = _db.GetFileLocationByNameAsync(LocationName.Prepared.ToString());
            System.IO.File.WriteAllText($@"{path.Result[0].Path}{client.ClientId}_{DateTime.Now:yyyyMMdd-HHmmss}_{client.DropFileName}", stringBuilder.ToString());
        }
    }
}
