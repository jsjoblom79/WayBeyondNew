using Renci.SshNet;
using Renci.SshNet.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class Transfer : ITransfer
    {
        private IBeyondRepository _db = new BeyondRepository();
        public Task<List<FileObject>> GetFileObjectsAsync(FileLocation location)
        {

            if(location.FileType == FileType.LOCAL)
            {
                return Task.FromResult(GetLocalFiles(location));
            }
            else
            {
                return GetRemoteFiles(location);
            }
        }

        public async Task<FileObject> DownloadFileAsync(FileObject file)
        {

            using (SftpClient client = new SftpClient(GetConnectionInfo(file.RemoteConnection)))
            {
                client.Connect();
                if(client.IsConnected)
                {
                    //Create a file stream to read the contents of the download file to.
                    var location = await _db.GetFileLocationByNameAsync(LocationName.DownloadLocation.ToString());
                    using (Stream fileStream = System.IO.File.OpenWrite($@"{location[0].Path}{file.FileName}"))
                    { 
                       await client.DownloadAsync(file.FullPath, fileStream);
                        return new FileObject
                        {
                            FileName = file.FileName,
                            FullPath = $"{location[0].Path}{file.FileName}",
                            FileType = FileType.LOCAL,
                            CreateDate = file.CreateDate
                        };
                    }
                }
                else { return null; }
               
            }

            
        }

        #region PrivateMethods
        private async Task<List<FileObject>> GetRemoteFiles(FileLocation location)
        {
            using (SftpClient client = new SftpClient(GetConnectionInfo(location.RemoteConnection)))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    List<FileObject> files = new List<FileObject>();
                    foreach (var file in await client.ListDirectoryAsync(location.Path))
                    {
                        if (!file.IsDirectory)
                        {
                            files.Add(new FileObject
                            {
                                FileName = file.Name,
                                CreateDate = file.LastWriteTime,
                                FileType = FileType.REMOTE,
                                FullPath = file.FullName,
                                RemoteConnection = location.RemoteConnection
                            });
                        }
                    }
                    return files;
                }
                else
                {
                    return new List<FileObject>();
                }
            }
        }
        
        private List<FileObject> GetLocalFiles(FileLocation location)
        {
            List<FileObject> files = new List<FileObject>();

            foreach (var file in Directory.GetFiles(location.Path))
            {
                files.Add(new FileObject
                {
                    FileName = Path.GetFileName(file),
                    CreateDate = System.IO.File.GetCreationTime(file),
                    FileType = FileType.LOCAL,
                    FullPath = file
                });
            }
            return files;
        }

        private ConnectionInfo GetConnectionInfo(RemoteConnection remote)
            => new ConnectionInfo(remote.Host,(int)remote.Port,remote.UserName,
                new PasswordAuthenticationMethod(remote.UserName,remote.Password));


        #endregion
    }
}
