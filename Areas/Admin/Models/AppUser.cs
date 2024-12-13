using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<BasketItem> BasketItems { get; set; }

    }
}
