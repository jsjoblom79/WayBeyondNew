using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class ProcessedFileBatch
{
    public long Id { get; set; }

    public string? BatchName { get; set; }

    public string? CreateDate { get; set; }

    public string? UpdateDate { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ICollection<ClientLoad> ClientLoads { get; set; } = new List<ClientLoad>();
}
