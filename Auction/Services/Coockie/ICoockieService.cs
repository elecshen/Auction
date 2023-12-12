namespace Auction.Services.Coockie
{
    public interface ICoockieService
    {
        public void RefreshCoockie(HttpContext context);

        public void AddCoockie(HttpContext context);

        public void RemoveCoockie(HttpContext context);
    }
}
