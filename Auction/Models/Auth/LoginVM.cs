using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Auction.Models.Auth
{
    public class LoginVM
    {
        [DisplayName("Логин")]
        //Validation
        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        public string Username { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        //Validation
        [Required(ErrorMessage = "Поле \"{0}\" должно быть заполнено.")]
        public string Password { get; set; } = null!;

        [DisplayName("Запомнить меня?")]
        public bool RememberMe { get; set; } = false;
    }
}
