using System;
using System.Collections.Generic;

namespace AutoSellBilet.Hardik.Model;

public partial class Kino
{
    public string Name { get; set; } = null!;

    public long? NomerZala { get; set; }

    public DateTime? Vrema { get; set; }

    public virtual Zal? NomerZalaNavigation { get; set; }

    public virtual ICollection<Sean> Seans { get; set; } = new List<Sean>();
}
