using System;
using System.Collections.Generic;

namespace AutoSellBilet.Hardik.Model;

public partial class Sean
{
    public int Id { get; set; }

    public string MovieName { get; set; } = null!;

    public long HallId { get; set; }

    public DateTime StartTime { get; set; }

    public decimal BasePrice { get; set; }

    public virtual Zal Hall { get; set; } = null!;

    public virtual Kino MovieNameNavigation { get; set; } = null!;

    public virtual ICollection<SeansMestum> SeansMesta { get; set; } = new List<SeansMestum>();
}
