using System.ComponentModel.DataAnnotations;

namespace Auction.Models.Lots
{
    public class CreateLotVM
    {
        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Название")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Некорректная длина")]
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
