using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public interface ITransfer
    {
        Task<List<FileObject>> GetFileObjectsAsync(FileLocation location);
        Task<FileObject> DownloadFileAsync(FileObject path, LocationName downloadLocation = LocationName.DownloadLocation);
        Task<bool> ArchiveFileAsync(FileObject path);
        Task<bool> UploadFile(FileObject path);
        Task<bool> DeleteFileAsync(FileObject path);

        // TexasTech Reporting Specific Tasks
        Task<string[]> GetNewFiles(string writeFolder);

        Task GetExceptionFilesAsync();
    }
}
