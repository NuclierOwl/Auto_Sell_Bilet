using System;
using System.Collections.Generic;

namespace AutoSellBilet.Dao.Model;

public partial class Mestum
{
    public int Id { get; set; }

    public long ZalId { get; set; }

    public int RowNumber { get; set; }

    public int MestoNumber { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<SeansMestum> SeansMesta { get; set; } = new List<SeansMestum>();

    public virtual Zal Zal { get; set; } = null!;
}
