using System;
using System.Collections.Generic;

namespace Auction.Models.MSSQLModels.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Lot> Lots { get; set; } = new List<Lot>();

    public virtual Role Role { get; set; } = null!;
}
