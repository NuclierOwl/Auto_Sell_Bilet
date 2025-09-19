using System;
using System.Collections.Generic;

namespace AutoSellBilet.Dao.Model;

public partial class Bron
{
    public Guid? Zritel { get; set; }

    public string Film { get; set; } = null!;

    public long? Mesto { get; set; }

    public string? Status { get; set; }

    public decimal Prise { get; set; }

    public int Id { get; set; }

    public virtual ICollection<Bilet> Bilets { get; set; } = new List<Bilet>();

    public virtual Kino? FilmNavigation { get; set; }

    public virtual User? ZritelNavigation { get; set; }
}
