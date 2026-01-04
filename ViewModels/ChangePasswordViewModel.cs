using System.ComponentModel.DataAnnotations;

namespace WmsCore.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Operator")]
        [Required(ErrorMessage = "Nazwa operatora jest wymagana")]
        public string UserName { get; set; }

        [Display(Name = "Nowe hasło")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Hasło musi mieć minimalnie {2} znaków")]
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Hasła muszą się zgadzać")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Powtórzone hasło jest wymagane")]
        [Display(Name = "Powtórz nowe hasło")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
