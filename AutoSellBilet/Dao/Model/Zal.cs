using System;
using System.Collections.Generic;

namespace AutoSellBilet.Dao.Model;

public partial class Zal
{
    public long Nomer { get; set; }

    public long? Maxmesto { get; set; }

    public virtual ICollection<Kino> Kinos { get; set; } = new List<Kino>();

    public virtual ICollection<Mestum> Mesta { get; set; } = new List<Mestum>();

    public virtual ICollection<Sean> Seans { get; set; } = new List<Sean>();
}
