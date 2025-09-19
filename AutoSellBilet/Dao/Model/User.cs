using System;
using System.Collections.Generic;

namespace AutoSellBilet.Dao.Model;

public partial class User
{
    public string? Name { get; set; }

    public string? Password { get; set; }

    public Guid Guid { get; set; }

    public virtual ICollection<Bilet> Bilets { get; set; } = new List<Bilet>();

    public virtual ICollection<Bron> Brons { get; set; } = new List<Bron>();
}
