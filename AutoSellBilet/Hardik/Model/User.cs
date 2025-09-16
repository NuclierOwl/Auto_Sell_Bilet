using System;
using System.Collections.Generic;

namespace AutoSellBilet.Hardik.Model;

public partial class User
{
    public string? Name { get; set; }

    public string? Password { get; set; }

    public Guid Guid { get; set; }

    public virtual ICollection<Bilet> Bilets { get; set; } = new List<Bilet>();
}
