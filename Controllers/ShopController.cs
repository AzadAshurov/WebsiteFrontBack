using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index(int id)
        {
            return View();
        }
    }
}
