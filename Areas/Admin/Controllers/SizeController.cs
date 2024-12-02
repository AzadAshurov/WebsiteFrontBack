using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels.Sizes;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetSizeAdminVM> SizesList = await _context.Sizes.Include(a => a.ProductSizes).Select(x => new GetSizeAdminVM { Id = x.Id, Name = x.Name, ProductSizes = x.ProductSizes }).ToListAsync();
            return View(SizesList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int? id, GetSizeAdminVM SizeVm)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }
            bool isExist = await _context.Sizes.AnyAsync(c => c.Name.Trim().ToLower() == SizeVm.Name.Trim().ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This Size already exists");
                return View();
            }
            Size Size = new Size
            {
                CreatedAt = DateTime.Now,
                Name = SizeVm.Name,
                IsDeleted = false
            };
            await _context.Sizes.AddAsync(Size);
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
            Size Size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (Size is null) return NotFound();
            UpdateSizeVM SizeVM = new UpdateSizeVM
            {
                Name = Size.Name
            };
            return View(SizeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSizeVM SizeVM)
        {
            if (id == null || id < 1) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View();
            }
            Size Size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (Size is null) return NotFound();
            if (_context.Sizes.Any(x => x.Name == SizeVM.Name && x.Id != SizeVM.Id))
            {
                ModelState.AddModelError(nameof(UpdateSizeVM.Name), "Size must be unique");
                return View(SizeVM);
            }
            Size.Name = SizeVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Size Size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (Size is null) return NotFound();
            _context.Sizes.Remove(Size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();
            Size Size = await _context.Sizes.Include(t => t.ProductSizes).FirstOrDefaultAsync(s => s.Id == id);
            if (Size is null) return NotFound();

            DetailSizeVM detailSize = new DetailSizeVM
            {
                Name = Size.Name,
                IsDeleted = Size.IsDeleted,
                CreatedAt = Size.CreatedAt,
                ProductSizes = Size.ProductSizes
            };

            return View(detailSize);
        }
    }
}
