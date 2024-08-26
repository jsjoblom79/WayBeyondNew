using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.Data.Models
{
    public class FileObject : IComparable<FileObject>
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public DateTime CreateDate { get; set; }
        public FileType FileType { get; set; }
        public RemoteConnection? RemoteConnection { get; set; }

        public int CompareTo(FileObject? other) => other.CreateDate.CompareTo(this.CreateDate);
    }

    public enum FileType
    {
        [Description("Local File")]
        LOCAL = 1,
        [Description("Remote File")]
        REMOTE = 2
    }
}
