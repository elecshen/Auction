using System.ComponentModel.DataAnnotations;

namespace Auction.Models.Lots
{
    public class EditLotVM
    {
        public int Pid { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Display(Name = "Название")]
        public string Title { get; set; } = null!;

        [Display(Name = "Описание")]
        [DataType(DataType.Text)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Дата начала ставок")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Дата окончания ставок")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiresOn { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Цена \"Купить сейчас\"")]
        [DataType(DataType.Currency)]
        public int BlitzPrice { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Стартовая цена")]
        [DataType(DataType.Currency)]
        public int StartPrice { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Шаг цены")]
        [DataType(DataType.Currency)]
        public int PriceStep { get; set; }
    }
}
