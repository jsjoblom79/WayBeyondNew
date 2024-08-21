using Renci.SshNet;
using Renci.SshNet.Async;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.Services
{
    public class Transfer : ITransfer
    {
        private IBeyondRepository _db = new BeyondRepository();

        public Task<List<FileObject>> GetFileObjectsAsync(FileLocation location)
        {

            if (location.FileType == FileType.LOCAL)
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
                if (client.IsConnected)
                {
                    //Create a file stream to read the contents of the download file to.
                    var location = await _db.GetFileLocationsByNameAsync(LocationName.DownloadLocation);
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
        public Task<bool> ArchiveFileAsync(FileObject path)
        {
            if (path.FileType == FileType.REMOTE)
            {
                var archiveDirectory = $"{Path.GetDirectoryName(path.FullPath)}/Archive/{path.FileName}".Replace("\\", "/");
                using (SftpClient client = new SftpClient(GetConnectionInfo(path.RemoteConnection)))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        var file = client.Get(path.FullPath);
                        file.MoveTo(archiveDirectory);
                    }
                }
                return Task.FromResult(true);
            }
            else
            {
                try
                {
                    var aDirectory = $"{Path.GetDirectoryName(path.FullPath)}\\Archive\\{path.FileName}";
                    if (!System.IO.File.Exists(aDirectory))
                        System.IO.File.Move(path.FullPath, aDirectory);

                    return Task.FromResult(true);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message,"IO Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return Task.FromResult(false);
                }
                
            }
        }

        public async Task<bool> UploadFile(FileObject path)
        {
            try
            {
                FileLocation location = _db.GetFileLocationsByNameAsync(LocationName.Drop).Result.First();
                
                string[] fileNames = path.FileName.Split('_');
                var connectInfo = GetConnectionInfo(location.RemoteConnection);
                using (SftpClient client = new SftpClient(connectInfo))
                {
                    client.Connect();
                    if(client.IsConnected)
                    {
                        using (FileStream stream = System.IO.File.OpenRead(path.FullPath))
                        {
                            await client.UploadAsync(stream, $"{location.Path}{fileNames[2]}");
                        }
                        client.Disconnect();
                        return true;
                    }
                    else
                    {
                        Log.Information($"Client Connection not made to {connectInfo.Host}");
                        return false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                
                Log.Error(ex.StackTrace);
                return false;
            }
        }
        public Task<bool> DeleteFileAsync(FileObject path)
        {
            try
            {
                System.IO.File.Delete(path.FullPath);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
                return Task.FromResult(false);
            }
            
        }
        #region PrivateMethods
        private async Task<List<FileObject>> GetRemoteFiles(FileLocation location)
        {
            using (SftpClient client = new SftpClient(GetConnectionInfo(location.RemoteConnection)))
            {
                try
                {
                    client.Connect();
                }
                catch (Exception ex)
                {
                    Log.Error($"{ex.Message} using Location: {location.FileLocationName}", ex);
                }
                
                if (client.IsConnected)
                {
                    List<FileObject> files = new List<FileObject>();
                    foreach (var file in await client.ListDirectoryAsync(location.Path))
                    {
                        if (!file.IsDirectory && file.LastWriteTime > DateTime.Now.AddDays(-31))
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

        public async Task<string[]> GetNewFiles(string writeFolder)
        {
            string[] files = new string[2];
            var folderPath = await _db.GetFileLocationsByNameAsync(LocationName.TexasTechMonthlyInput);
            string activeWC = $"TT_ACTIVE_INV_new_{DateTime.Now.Year}-{DateTime.Now:MM}-01";
            string inactiveWC = $"TT_CANCELLED_PIF_{DateTime.Now.Year}-{DateTime.Now:MM}-01";

            foreach (var file in folderPath)
            {
                using (SftpClient client = new SftpClient(GetConnectionInfo(file.RemoteConnection)))
                {
                    client.Connect();
                    int i = 0;
                    var fileList = client.ListDirectory(file.Path).Where(f => f.LastWriteTime.Month == DateTime.Now.Month && f.LastWriteTime.Year == DateTime.Now.Year && f.Name.StartsWith("TT")).ToList();
                    foreach (var item in fileList)
                    {
                        if (item.Name.Contains(activeWC) || item.Name.Contains(inactiveWC))
                        {
                            var pathLocation = $"{writeFolder}{Path.GetFileName(item.FullName).Replace(":", "")}";
                            using (Stream stream = System.IO.File.OpenWrite(pathLocation))
                            {
                                client.DownloadFile(item.FullName, stream);
                                files[i] = Path.GetFileName(item.FullName).Replace(":", "");
                                i++;
                                
                            }
                        }
                    }
                }
            }
            return files;
        }








        #endregion
    }
}
