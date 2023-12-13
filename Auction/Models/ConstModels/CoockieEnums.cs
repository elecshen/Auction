namespace Auction.Models.ConstModels
{
    public static class CoockieEnums
    {
        public class CoockieBase(string key, string defaultValue, List<string> values)
        {
            public string Key { get; } = key;
            public string Default { get; } = defaultValue;
            public List<string> Values { get; } = values;
        }

        public class Theme : CoockieBase
        {
            public const string Light = "light";
            public const string Dark = "dark";

            public Theme() : base("theme", Light, [Light, Dark])
            {
            }
        }

        public static Theme ThemeObject { get; } = new();


        public static IEnumerable<Type> DerivedTypes { get => typeof(CoockieBase).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CoockieBase))); }
    }
}
