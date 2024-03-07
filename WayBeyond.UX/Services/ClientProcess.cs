using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;


namespace WayBeyond.UX.Services
{
    public class ClientProcess : IClientProcess
    {
        private ITransfer _transfer;
        public ClientProcess(ITransfer transfer)
        {
            _transfer = transfer;   
        }
        public async Task<bool> ProcessClientFile(FileObject file, Client client)
        {
            FileObject downloadedFile = file;
            if (file.FileType == FileType.REMOTE)
            {
                downloadedFile = await _transfer.DownloadFileAsync(file);
            }
            
            ExcelService excelService = new ExcelService();
            excelService.ReadClientFile(client, downloadedFile);


            //TODO: Write drop file

            //TODO: Archive local file and Remote file.

            


            return true;
        }
    }
}
