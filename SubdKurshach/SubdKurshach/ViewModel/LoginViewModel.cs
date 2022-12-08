using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Проверьте заполненность логина или пароля")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Проверьте заполненность логина или пароля")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
