using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels.Colors;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetColorAdminVM> ColorsList = await _context.Colors.Include(a => a.ProductColors).Select(x => new GetColorAdminVM { Id = x.Id, Name = x.Name, ProductColors = x.ProductColors }).ToListAsync();
            return View(ColorsList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int? id, GetColorAdminVM ColorVm)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }
            bool isExist = await _context.Colors.AnyAsync(c => c.Name.Trim().ToLower() == ColorVm.Name.Trim().ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This Color already exists");
                return View();
            }
            Color Color = new Color
            {
                CreatedAt = DateTime.Now,
                Name = ColorVm.Name,
                IsDeleted = false
            };
            await _context.Colors.AddAsync(Color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View();
            }
            Color Color = await _context.Colors.FirstOrDefaultAsync(s => s.Id == id);
            if (Color is null) return NotFound();
            UpdateColorVM ColorVM = new UpdateColorVM
            {
                Name = Color.Name
            };
            return View(ColorVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateColorVM ColorVM)
        {
            if (id == null || id < 1) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View();
            }
            Color Color = await _context.Colors.FirstOrDefaultAsync(s => s.Id == id);
            if (Color is null) return NotFound();
            if (_context.Colors.Any(x => x.Name == ColorVM.Name && x.Id != ColorVM.Id))
            {
                ModelState.AddModelError(nameof(UpdateColorVM.Name), "Color must be unique");
                return View(ColorVM);
            }
            Color.Name = ColorVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Color Color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (Color is null) return NotFound();
            _context.Colors.Remove(Color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Color Color = await _context.Colors.Include(t => t.ProductColors).FirstOrDefaultAsync(s => s.Id == id);
            if (Color is null) return NotFound();
            DetailColor detailColor = new DetailColor
            {
                Name = Color.Name,
                IsDeleted = Color.IsDeleted,
                CreatedAt = Color.CreatedAt,
                ProductColors = Color.ProductColors
            };

            return View(detailColor);
        }
    }
}
