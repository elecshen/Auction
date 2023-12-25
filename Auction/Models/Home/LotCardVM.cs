using Auction.Models.MSSQLModels.Entities;
using static Auction.Models.ConstModels.LotStatus;

namespace Auction.Models.Home
{
    public class LotCardVM
    {
        public long PublicId { get; set; }
        public int CurrentPrice { get; set; }
        public int BlitzPrice { get; set; }
        public string Title { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string CategoryName { get; set; } = null!;

        public LotCardVM(Lot l)
        {
            PublicId = l.PublicId;
            CurrentPrice = l.LastBid is null ? l.StartPrice : l.LastBid.Value;
            BlitzPrice = l.BlitzPrice;
            Title = l.Title;
            StartDate = l.StartDate;
            ExpiresOn = l.StartDate.AddSeconds(l.Interval);
            CategoryName = l.Category.Name;

            Status = DateTime.Now < ExpiresOn && !l.IsCompleted ? InProgress : Completed;
        }
    }
}
