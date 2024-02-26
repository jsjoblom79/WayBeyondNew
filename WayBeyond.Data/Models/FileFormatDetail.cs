using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class FileFormatDetail
{
    public long Id { get; set; }

    public string? Field { get; set; }

    public string? FileColumn { get; set; }

    public string? ColumnType { get; set; }

    public string? SpecialCase { get; set; }

    public long? FileFormatId { get; set; }

    public virtual FileFormat? FileFormat { get; set; }
}
