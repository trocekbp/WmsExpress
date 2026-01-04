using Microsoft.AspNetCore.Identity;

namespace WmsCore.Models
{
    public class User: IdentityUser
    {
        public string FullName { get; set; }
    }
}
