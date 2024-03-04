using Renci.SshNet;
using Renci.SshNet.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class Transfer : ITransfer

    {
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
                                FullPath = file.FullName
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
        #region PrivateMethods
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
