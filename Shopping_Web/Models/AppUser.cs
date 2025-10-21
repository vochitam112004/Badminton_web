using Microsoft.AspNetCore.Identity;

namespace Shopping_Web.Models
{
    public class AppUser : IdentityUser
    { 
        public string Ocupation { get; set; }
    }
}
