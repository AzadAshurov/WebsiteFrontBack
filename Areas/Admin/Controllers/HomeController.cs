using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels.Orders;
using WebApplication1.DAL;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<ShowOrderVM> showOrdersVM = await _context.Orders
         .Include(o => o.OrderItems)
         .Include(o => o.AppUser)
         .Select(o => new ShowOrderVM
         {
             Address = o.Address,
             TotalPrice = o.OrderItems.Sum(oi => oi.Price * oi.Count),
             AppUserId = o.AppUserId,
             AppUser = o.AppUser,
             OrderItems = o.OrderItems.ToList(),
             Status = o.Status,
             CreatedAt = o.CreatedAt
         })
         .ToListAsync();


            return View(showOrdersVM);
        }
        [HttpPost]
        public IActionResult Index(string name)
        {
            return View();
        }
    }
}
