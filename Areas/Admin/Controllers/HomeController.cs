using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string name)
        {
            return View();
        }
    }
}
