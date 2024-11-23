using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ShopController : Controller
    {
        private AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int id)
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Product? product = _context.Products
                .Include(p => p.ProductImages.OrderByDescending(pi => pi.IsPrimary))
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product is null) return NotFound();

            DetailVM detailVM = new DetailVM
            {
                Products = product,
                RelatedProducts = _context.Products
                    .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                    .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null))
                    .ToList()
            };

            return View(detailVM);
        }
    }
}
