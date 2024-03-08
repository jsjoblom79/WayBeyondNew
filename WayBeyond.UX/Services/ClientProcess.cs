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
        public async Task<bool> ProcessClientFile(FileObject file, Client client)
        {
            FileObject downloadedFile = file;
            if (file.FileType == FileType.REMOTE)
            {
                downloadedFile = await _transfer.DownloadFileAsync(file);
            }
            
            ExcelService excelService = new ExcelService();

            WriteDropFile(client, await excelService.ReadClientFile(client, downloadedFile));

            if (file.RemoteConnection != null) _transfer.ArchiveFileAsync(file);
            
            _transfer.ArchiveFileAsync(downloadedFile);

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
                    stringBuilder.Append(debtor.GetType().GetProperty(detail.Field).GetValue(debtor));
                    stringBuilder.Append('\t');
                }
                stringBuilder.AppendLine();
            }
            var path = _db.GetFileLocationByNameAsync(LocationName.Prepared.ToString());
            System.IO.File.WriteAllText($@"{path}{client.ClientId}_{DateTime.Now:yyyyMMdd-HHmmss}_{client.DropFileName}", stringBuilder.ToString());
        }
    }
}
