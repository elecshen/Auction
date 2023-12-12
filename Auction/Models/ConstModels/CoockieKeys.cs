namespace Auction.Models.ConstModels
{
    public static class CoockieKeys
    {
        public static class Theme
        {
            public const string Key = "theme";
            public static readonly string[] Values = ["light", "dark"];
            public static readonly string DefaultValue = Values[0];
        }
    }
}
