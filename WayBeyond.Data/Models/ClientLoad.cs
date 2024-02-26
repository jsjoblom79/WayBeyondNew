using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class ClientLoad
{
    public long Id { get; set; }

    public long? ClientId { get; set; }

    public string? ClientName { get; set; }

    public long? Balance { get; set; }

    public long? DebtorCount { get; set; }

    public string? CreateDate { get; set; }

    public string? FileName { get; set; }

    public string? DateOnLoadFile { get; set; }

    public string? Comments { get; set; }

    public long? DropNumber { get; set; }

    public long? ProcessedFileBatchId { get; set; }

    public virtual ProcessedFileBatch? ProcessedFileBatch { get; set; }
}
