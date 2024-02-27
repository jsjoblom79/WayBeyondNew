using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class RemoteConnection
{
    public long Id { get; set; }

    public string? Host { get; set; }

    public long? Port { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public long? FingerprintRequired { get; set; }

    public string? FingerPrint { get; set; }

    public string? Name { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpatedDate { get; set; }

    public virtual ICollection<FileLocation> FileLocations { get; set; } = new List<FileLocation>();
}
