using System;
using System.Collections.Generic;

namespace Auction.Models.MSSQLModels.Entities;

public partial class Bid
{
    public Guid Id { get; set; }

    public int Value { get; set; }

    public DateTime BidDate { get; set; }

    public Guid BidderId { get; set; }

    public Guid LotId { get; set; }

    public virtual User Bidder { get; set; } = null!;

    public virtual Lot Lot { get; set; } = null!;

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();
}
