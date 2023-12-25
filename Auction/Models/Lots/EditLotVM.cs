using System.ComponentModel.DataAnnotations;

namespace Auction.Models.Lots
{
    public class EditLotVM
    {
        [Required]
        public int Pid { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Display(Name = "Название")]
        public string Title { get; set; } = null!;

        [Display(Name = "Описание")]
        [DataType(DataType.Text)]
        public string? Description { get; set; }

        [Display(Name = "Дата начала торгов")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(0, 30)]
        public int Days { get; set; } = 0;
        [Required]
        [Range(0, 23)]
        public int Hours { get; set; } = 1;
        [Required]
        [Range(0, 59)]
        public int Minutes { get; set; } = 0;
        [Required]
        [Range(0, 59)]
        public int Seconds { get; set; } = 0;

        [Display(Name = "Стартовая цена")]
        public int StartPrice { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Цена \"Купить сейчас\"")]
        [Range(1, int.MaxValue, ErrorMessage = "Цена \"Купить сейчас\" должна быть положительной")]
        public int BlitzPrice { get; set; }
    }
}
