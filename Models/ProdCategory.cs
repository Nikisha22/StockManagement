using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class ProdCategory
{
    public short PcId { get; set; }

    public string PcName { get; set; } = null!;

    public string PcDescription { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
