using System.ComponentModel;

namespace Auction.Models.Lots
{
    public class BidVM
    {
        public string BidderName { get; set; } = null!;
        public int Value { get; set; }
        public DateTime BidDate { get; set; }
    }
}
