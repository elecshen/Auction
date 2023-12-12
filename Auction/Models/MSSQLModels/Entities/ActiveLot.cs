using System;
using System.Collections.Generic;

namespace Auction.Models.MSSQLModels.Entities;

public partial class ActiveLot
{
    public string Title { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime ExpiresOn { get; set; }

    public int? BidCount { get; set; }
}
