using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Areas.Admin.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
