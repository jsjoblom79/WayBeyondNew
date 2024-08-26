using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class FileLocation
{
    public long Id { get; set; }

    public string? FileLocationName { get; set; }

    public string? Path { get; set; }

    public FileType FileType { get; set; }

    public long? RemoteConnectionId { get; set; }

    public virtual RemoteConnection? RemoteConnection { get; set; }
}
