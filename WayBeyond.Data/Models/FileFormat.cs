using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class FileFormat
{
    public long Id { get; set; }

    public long? ClientId { get; set; }

    public string? FileFormatName { get; set; }

    public string? CreateDate { get; set; }

    public string? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<FileFormatDetail> FileFormatDetails { get; set; } = new List<FileFormatDetail>();
}
