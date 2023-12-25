namespace Auction.Models.ConstModels
{
    public static class ModelErrors
    {
        public class ModelError(string key, string errorMessage)
        {
            public readonly string Key = key;
            public readonly string ErrorMessage = errorMessage;
        }

        public static ModelError Bid_Expired { get; } = new(nameof(MSSQLModels.Entities.Bid.BidDate), "Лот закрыт: время вышло");
        public static ModelError Bid_IncorrectValue { get; } = new(nameof(MSSQLModels.Entities.Bid.Value), "Значение ставки некорректно");
    }
}
