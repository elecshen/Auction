using Auction.Models.MSSQLModels.Entities;

namespace Auction.Models.Home
{
    public class LotCardVM
    {
        public int StartPrice { get; set; }
        public Bid? LastBid { get; set; }
        public int BlitzPrice { get; set; }
        public string Title { get; set; } = null!;
        public string StatusName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
