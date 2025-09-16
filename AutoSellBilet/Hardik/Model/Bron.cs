using System;
using System.Collections.Generic;

namespace AutoSellBilet.Hardik.Model;

public partial class Bron
{
    public Guid? Zritel { get; set; }

    public string? Film { get; set; }

    public double? Mesto { get; set; }

    public string? Status { get; set; }

    public decimal Prise { get; set; }

    public virtual Kino? FilmNavigation { get; set; }

    public virtual User? ZritelNavigation { get; set; }
}
