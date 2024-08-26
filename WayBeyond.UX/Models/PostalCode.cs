using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class PostalCode
{
    public long? Zip { get; set; }

    public double? Lat { get; set; }

    public double? Lng { get; set; }

    public string? City { get; set; }

    public string? StateId { get; set; }

    public string? StateName { get; set; }
}
