using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class DropFormatDetail
{
    public long Id { get; set; }

    public string? Field { get; set; }

    public long? Position { get; set; }

    public long? FieldType { get; set; }

    public long? DropFormatId { get; set; }

    public virtual DropFormat? DropFormat { get; set; }
}
