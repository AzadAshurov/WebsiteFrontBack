using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            var slides = await _context.Slides.OrderBy(x => x.Order).Take(3).ToListAsync();
            var products = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();

            Console.WriteLine($"Slides count: {slides.Count}");
            Console.WriteLine($"Products count: {products.Count}");

            HomeVM homeVM = new HomeVM
            {
                Slides = slides,
                Products = products
            };
            return View(homeVM);
        }

    }
}
