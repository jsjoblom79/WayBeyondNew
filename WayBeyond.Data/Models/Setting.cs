using System;
using System.Collections.Generic;

namespace WayBeyond.Data.Models;

public partial class Setting
{
    public long Id { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }
}
