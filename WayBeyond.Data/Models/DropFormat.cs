using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class DropFormat
{
    public long Id { get; set; }

    public long? DropId { get; set; }

    public string? DropName { get; set; }

    public string? CreateDate { get; set; }

    public string? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<DropFormatDetail> DropFormatDetails { get; set; } = new List<DropFormatDetail>();
}
