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
    }
}
