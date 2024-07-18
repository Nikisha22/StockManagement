using System;
using System.Collections.Generic;

namespace StockManagement.Models;

public partial class Supplier
{
    public short SId { get; set; }

    public string SName { get; set; } = null!;

    public string SAddress { get; set; } = null!;

    public string SPhone { get; set; } = null!;

    public string SEmail { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
