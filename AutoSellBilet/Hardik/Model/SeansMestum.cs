using System;
using System.Collections.Generic;

namespace AutoSellBilet.Hardik.Model;

public partial class SeansMestum
{
    public int Id { get; set; }

    public int SeansId { get; set; }

    public int MestoId { get; set; }

    public string? Status { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Bilet> Bilets { get; set; } = new List<Bilet>();

    public virtual Mestum Mesto { get; set; } = null!;

    public virtual Sean Seans { get; set; } = null!;
}
