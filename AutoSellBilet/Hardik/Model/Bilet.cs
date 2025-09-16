using System;
using System.Collections.Generic;

namespace AutoSellBilet.Hardik.Model;

public partial class Bilet
{
    public int Id { get; set; }

    public int SeansMestaId { get; set; }

    public Guid ZritelGuid { get; set; }

    public string? Status { get; set; }

    public virtual SeansMestum SeansMesta { get; set; } = null!;

    public virtual User Zritel { get; set; } = null!;
}
