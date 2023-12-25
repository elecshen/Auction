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
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Название лота дожнобыть от \"{2}\" до \"{1}\" символов")]
        public string Title { get; set; } = null!;

        [Display(Name = "Описание")]
        [DataType(DataType.Text)]
        public string? Description { get; set; }

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

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Стартовая цена")]
        [Range(1, int.MaxValue, ErrorMessage = "Стартовая цена должна быть положительной")]
        public int StartPrice { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Display(Name = "Цена \"Купить сейчас\"")]
        [Range(1, int.MaxValue, ErrorMessage = "Цена \"Купить сейчас\" должна быть положительной")]
        public int BlitzPrice { get; set; }
    }
}
