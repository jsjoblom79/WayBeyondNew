using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class Client
{
    public long Id { get; set; }

    public long? ClientId { get; set; }

    public string? ClientName { get; set; }

    public long? DropNumber { get; set; }

    public string? DropFileName { get; set; }

    public string? AssemblyName { get; set; }

    public long? DropFormatId { get; set; }

    public long? FileFormatId { get; set; }

    public virtual DropFormat? DropFormat { get; set; }

    public virtual FileFormat? FileFormat { get; set; }
}
