using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Auction.Models.Auth
{
    public class RegisterVM
    {
        [DisplayName("Логин")]
        //Validation
        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "\"{0}\" должен быть длинной не менее {2} и не более {1} символов.")]
        public string Username { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        //Validation
        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "\"{0}\" должен быть длинной не менее {2} сиволов.")]
        [RegularExpression("^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$", ErrorMessage = "\"{0}\" должен иметь хотя бы одну цифру и один строчный и прописной символ.")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        //Validation
        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        [Compare($"{nameof(Password)}", ErrorMessage = "Пароли не совпадают")]
        public string RepeatPassword { get; set; } = null!;
    }
}
