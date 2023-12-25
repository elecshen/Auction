namespace Auction.Models.Lots
{
    public class LotVM
    {
        public long PublicId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int CurrentPrice { get; set; }
        public int NextMinPrice { get; set; }
        public int BlitzPrice { get; set; }
        public string Status { get; set; } = null!;
        public bool IsAuthenticated { get; set; }
        public bool IsOwner { get; set; }
        public bool IsCanBid { get; set; }
        public List<BidVM> Bids { get; set; } = [];
    }
}
