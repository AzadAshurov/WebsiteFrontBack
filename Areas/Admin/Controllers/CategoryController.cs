using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Category.Include(x => x.Product).ToListAsync();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }
            bool isExist = await _context.Category.AnyAsync(c => c.Name.Trim().ToLower() == category.Name.Trim().ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }

            category.CreatedAt = DateTime.Now;
            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (id == null || id < 1) return BadRequest();

            Category existed = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = await _context.Category.AnyAsync(c => c.Name.Trim() == category.Name.Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(Category.Name), "Category already exists");
                return View();
            }
            existed.Name = category.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Category.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Category.Include(pr => pr.Product).FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            return View(category);
        }
    }

}
