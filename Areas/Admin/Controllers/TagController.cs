using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels;
using WebApplication1.Areas.Admin.ViewModels.Tags;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetTagAdminVM> tagsList = await _context.Tags.Include(a => a.ProductTags).Select(x => new GetTagAdminVM { Id = x.Id, Name = x.Name, ProductTags = x.ProductTags }).ToListAsync();
            return View(tagsList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int? id, GetTagAdminVM tagVm)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }
            bool isExist = await _context.Tags.AnyAsync(c => c.Name.Trim().ToLower() == tagVm.Name.Trim().ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This Tag already exists");
                return View();
            }
            Tag tag = new Tag
            {
                CreatedAt = DateTime.Now,
                Name = tagVm.Name,
                IsDeleted = false
            };
            await _context.Tags.AddAsync(tag);
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
            Tag tag = await _context.Tags.FirstOrDefaultAsync(s => s.Id == id);
            if (tag is null) return NotFound();
            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name = tag.Name
            };
            return View(tagVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateTagVM tagVM)
        {
            if (id == null || id < 1) return BadRequest();

            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(s => s.Id == id);
            if (tag is null) return NotFound();
            tag.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (tag is null) return NotFound();
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
