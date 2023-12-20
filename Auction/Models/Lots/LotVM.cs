using Auction.Models.MSSQLModels.Entities;

namespace Auction.Models.Lots
{
    public class LotVM
    {
        public long PublicId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int StartPrice { get; set; }
        public Bid? LastBid { get; set; }
        public int PriceStep { get; set; }
        public int BlitzPrice { get; set; }
        public string StatusName { get; set; } = null!;
        public bool IsClosed { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string? Description { get; set; }
        public List<BidVM> Bids { get; set; } = new();
    }
}
