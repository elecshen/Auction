using System;
using System.Collections.Generic;

namespace Auction.Models.MSSQLModels.Entities;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool SetDefault { get; set; }

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
