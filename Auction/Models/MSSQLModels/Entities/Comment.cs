using System;
using System.Collections.Generic;

namespace Auction.Models.MSSQLModels.Entities;

public partial class Comment
{
    public Guid Id { get; set; }

    public string Text { get; set; } = null!;

    public DateTime Date { get; set; }

    public Guid CommentatorId { get; set; }

    public Guid LotId { get; set; }

    public virtual User Commentator { get; set; } = null!;

    public virtual Lot Lot { get; set; } = null!;
}
