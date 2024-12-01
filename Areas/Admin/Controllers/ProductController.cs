using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels;
using WebApplication1.Areas.Admin.ViewModels.Products;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<GetProductAdminVM> productsVMs = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .Select(p => new GetProductAdminVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    Image = p.ProductImages[0].Image
                })
                .ToListAsync();

            return View(productsVMs);
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Category.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Category.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "No such category,please select");
                return View(productVM);
            }
            if (productVM.TagIds is not null)
            {
                bool tagResult = productVM.TagIds.Any(x => !productVM.Tags.Exists(xx => xx.Id == x));
                if (tagResult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags were wrong");
                    return View(productVM);
                }
            }
            Product product = new()
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId.Value,
                Description = productVM.Description,
                Price = productVM.Price.Value,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            if (productVM.TagIds is not null)
            {
                product.ProductTags = productVM.TagIds.Select(x => new ProductTag { TagId = x }).ToList();
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Product product = await _context.Products.Include(t => t.ProductTags).FirstOrDefaultAsync(s => s.Id == id);

            if (product is null) return NotFound();

            UpdateProductVM updateProductVM = new()
            {
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Categories = _context.Category.ToList(),
                TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
                Tags = _context.Tags.ToList()
            };

            return View(updateProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == null || id < 1) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            Product productFromSQL = await _context.Products.FirstOrDefaultAsync(s => s.Id == id);

            if (productFromSQL is null) return NotFound();
            productVM.Categories = await _context.Category.ToListAsync();

            if (productFromSQL.CategoryId != productVM.CategoryId)
            {
                if (!productVM.Categories.Any(x => x.Id == productFromSQL.CategoryId))
                {
                    return View(productVM);
                }
            }
            productFromSQL.SKU = productVM.SKU;
            productFromSQL.Price = productVM.Price.Value;
            productFromSQL.CategoryId = productVM.CategoryId.Value;
            productFromSQL.Name = productVM.Name;
            productFromSQL.Description = productVM.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

    }
}
