namespace Auction.Models.ConstModels
{
    public static class LotStatus
    {
        public static string Idle { get; set; } = "Ожидает запуска";
        public static string InProgress { get; set; } = "Идут торги";
        public static string Completed { get; set; } = "Завершён";
        public static string Failed { get; set; } = "Не состоялся";
    }
}
