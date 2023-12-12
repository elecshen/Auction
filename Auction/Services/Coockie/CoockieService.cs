using Auction.Models.ConstModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Auction.Models.ConstModels.CoockieEnums;

namespace Auction.Services.Coockie
{
    public class CoockieService : ICoockieService
    {
        public void AddCoockie(HttpContext context, CoockieBase coockieProperty, string value)
        {
            context.Response.Cookies.Append(coockieProperty.Key, value, new() { MaxAge=TimeSpan.FromDays(30)});
        }

        public string? GetCoockie(HttpContext context, CoockieBase coockieProperty)
        {
            string? v = null;
            bool valueExist = 
                context.Request.Cookies.ContainsKey(coockieProperty.Key) 
                && context.Request.Cookies.TryGetValue(coockieProperty.Key, out v) 
                && v is not null;
            if (coockieProperty.Key == ThemeObject.Key)
            {
                if (valueExist && ThemeObject.Values.Contains(v!))
                    return v;
                AddCoockie(context, coockieProperty, ThemeObject.Default);
                return ThemeObject.Default;
            }
            else
            {
                return null;
            }
        }

        public void RefreshCoockie(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public void RemoveCoockie(HttpContext context, CoockieBase coockieProperty)
        {
            context.Response.Cookies.Delete(coockieProperty.Key);
        }
    }
}
