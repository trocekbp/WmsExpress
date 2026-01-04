using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WmsCore.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Operator")]
        [Required(ErrorMessage ="Nazwa operatora jest wymagana")]
        public string UserName { get; set; }

        [Display(Name = "Hasło")]
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }

    }
}
