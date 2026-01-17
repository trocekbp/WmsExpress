using System.ComponentModel.DataAnnotations;

namespace WmsCore.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Operator")]
        [Required(ErrorMessage = "Nazwa operatora jest wymagana")]
        public string UserName { get; set; }
        public string FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Hasło")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Hasło musi mieć minimalnie {2} znaków")]
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage ="Hasła muszą się zgadzać")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Powtórzone hasło jest wymagane")]
        [Display(Name = "Powtórz hasło")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Rola w systemie")]
        [Required(ErrorMessage = "Wybór roli jest wymagany")]
        public string UserRole { get; set; } = "User";
    }
}
