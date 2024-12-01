using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels;
using WebApplication1.DAL;

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
    }
}
