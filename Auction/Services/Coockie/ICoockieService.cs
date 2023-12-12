using static Auction.Models.ConstModels.CoockieEnums;

namespace Auction.Services.Coockie
{
    public interface ICoockieService
    {
        public void AddCoockie(HttpContext context, CoockieBase coockieProperty, string value);

        public string? GetCoockie(HttpContext context, CoockieBase coockieProperty);

        public void RefreshCoockie(HttpContext context);

        public void RemoveCoockie(HttpContext context, CoockieBase coockieProperty);
    }
}
