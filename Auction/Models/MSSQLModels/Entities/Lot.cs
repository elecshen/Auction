namespace Auction.Models.MSSQLModels.Entities;

public partial class Lot
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ExpiresOn { get; set; }

    public int BlitzPrice { get; set; }

    public int StartPrice { get; set; }

    public int PriceStep { get; set; }

    public Guid? LastBidId { get; set; }

    public Guid OwnerId { get; set; }

    public int StatusId { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Bid? LastBid { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
