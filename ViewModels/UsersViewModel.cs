using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WmsCore.ViewModels
{
    public class UsersViewModel
    {
        [DisplayName("Operator")]
        public string Username { get; set; }
        [DisplayName("Imię nazwisko")]
        public string Fullname { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Role")]
        public string Role { get; set; }
    }
}
