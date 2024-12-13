using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.ViewModels;
using WebApplication1.Areas.Admin.ViewModels.Products;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Utilities.Enums;
using WebApplication1.Utilities.Extensions;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
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
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Category.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                Colors = await _context.Colors.ToListAsync()
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Category.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
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
                    ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tags are wrong");
                    return View(productVM);
                }
            }
            if (productVM.SizeIds is not null)
            {
                bool sizeResult = productVM.SizeIds.Any(x => !productVM.Sizes.Exists(xx => xx.Id == x));
                if (sizeResult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.SizeIds), "Sizes are wrong");
                    return View(productVM);
                }
            }
            if (productVM.ColorIds is not null)
            {
                bool colorResult = productVM.ColorIds.Any(x => !productVM.Colors.Exists(xx => xx.Id == x));
                if (colorResult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.ColorIds), "Colors are wrong");
                    return View(productVM);
                }
            }
            if (!productVM.MainPhoto.IsFileTypeValid("image/"))
            {
                ModelState.AddModelError(nameof(productVM.MainPhoto), "File format is incorrect");
                return View(productVM);
            }
            if (!productVM.MainPhoto.IsFileSizeValid(FileSize.Megabyte, 2))
            {
                ModelState.AddModelError(nameof(productVM.MainPhoto), "Size of file must be below 2 mb");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.IsFileTypeValid("image/"))
            {
                ModelState.AddModelError(nameof(productVM.HoverPhoto), "File format is incorrect");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.IsFileSizeValid(FileSize.Megabyte, 2))
            {
                ModelState.AddModelError(nameof(productVM.HoverPhoto), "Size of file must be below 2 mb");
                return View(productVM);
            }

            ProductImage mainImage = new()
            {
                Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = true,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            ProductImage hoverImage = new()
            {
                Image = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = false,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            Product product = new()
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId.Value,
                Description = productVM.Description,
                Price = productVM.Price.Value,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ProductImages = new List<ProductImage>
                {
                    mainImage, hoverImage
                }
            };
            if (productVM.TagIds is not null)
            {
                product.ProductTags = productVM.TagIds.Select(x => new ProductTag { TagId = x }).ToList();
            }
            if (productVM.ColorIds is not null)
            {
                product.ProductColors = productVM.ColorIds.Select(x => new ProductColor { ColorId = x }).ToList();
            }
            if (productVM.SizeIds is not null)
            {
                product.ProductSizes = productVM.SizeIds.Select(x => new ProductSize { SizeId = x }).ToList();
            }
            string text = string.Empty;
            if (productVM.AdditionalPhotos is not null)
            {
                foreach (IFormFile file in productVM.AdditionalPhotos)
                {
                    if (!file.IsFileTypeValid("image/"))
                    {
                        text += $"<p class=\"text-warning\">{file.FileName} type was not correct</p>";
                        continue;
                    }

                    if (!file.IsFileSizeValid(FileSize.Megabyte, 1))
                    {
                        text += $"<p class=\"text-warning\">{file.FileName} size was not correct</p>";
                        continue;
                    }
                    product.ProductImages.Add(new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        IsPrimary = null
                    });
                }
                TempData["FileWarning"] = text;
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Product product = await _context.Products.Include(t => t.ProductSizes).Include(t => t.ProductImages).Include(t => t.ProductColors).Include(t => t.ProductTags).FirstOrDefaultAsync(s => s.Id == id);

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
                Tags = _context.Tags.ToList(),
                ColorIds = product.ProductColors.Select(x => x.ColorId).ToList(),
                Colors = _context.Colors.ToList(),
                SizeIds = product.ProductSizes.Select(x => x.SizeId).ToList(),
                Sizes = _context.Sizes.ToList(),
                ProductImages = product.ProductImages.ToList(),
            };

            return View(updateProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == null || id < 1) return BadRequest();


            Product productFromSQL = await _context.Products.Include(x => x.ProductImages).Include(x => x.ProductColors).Include(x => x.ProductSizes).Include(x => x.ProductTags).FirstOrDefaultAsync(s => s.Id == id);

            if (productFromSQL is null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.IsFileTypeValid("image/"))
                {
                    ModelState.AddModelError(nameof(productVM.MainPhoto), "File format is incorrect");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.IsFileSizeValid(FileSize.Megabyte, 2))
                {
                    ModelState.AddModelError(nameof(productVM.MainPhoto), "Size of file must be below 2 mb");
                    return View(productVM);
                }
                ProductImage mainImage = new()
                {
                    Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = true,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
            }
            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.IsFileTypeValid("image/"))
                {
                    ModelState.AddModelError(nameof(productVM.HoverPhoto), "File format is incorrect");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.IsFileSizeValid(FileSize.Megabyte, 2))
                {
                    ModelState.AddModelError(nameof(productVM.HoverPhoto), "Size of file must be below 2 mb");
                    return View(productVM);
                }
                ProductImage hoverImage = new()
                {
                    Image = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = false,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
            }





            productVM.Categories = await _context.Category.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.ProductImages = productFromSQL.ProductImages;
            if (productFromSQL.CategoryId != productVM.CategoryId)
            {
                if (!productVM.Categories.Any(x => x.Id == productFromSQL.CategoryId))
                {
                    return View(productVM);
                }
            }

            if (productVM.TagIds is not null)
            {
                bool tagResult = productVM.TagIds.Any(x => !productVM.Tags.Any(xx => xx.Id == x));
                if (tagResult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.TagIds), "Tags are incorrect");
                    return View(productVM);
                }
            }
            else
            {
                productVM.TagIds = new();
            }
            if (productVM.ColorIds is not null)
            {
                bool colorResult = productVM.ColorIds.Any(x => !productVM.Colors.Any(xx => xx.Id == x));
                if (colorResult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.ColorIds), "Colors are incorrect");
                    return View(productVM);
                }
            }
            else
            {
                productVM.ColorIds = new();
            }
            if (productVM.SizeIds is not null)
            {
                bool sizeResult = productVM.SizeIds.Any(x => !productVM.Sizes.Any(xx => xx.Id == x));
                if (sizeResult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SizeIds), "Sizes are incorrect");
                    return View(productVM);
                }
            }
            else
            {
                productVM.SizeIds = new();
            }

            _context.ProductSizes.RemoveRange(productFromSQL.ProductSizes.Where(x => !productVM.SizeIds.Exists(xr => xr == x.SizeId)));
            _context.ProductSizes.AddRange(productVM.SizeIds.Where(x => !productFromSQL.ProductSizes.Exists(xd => xd.SizeId == x)).ToList().Select(xz => new ProductSize { SizeId = xz, ProductId = productFromSQL.Id }));

            _context.ProductColors.RemoveRange(productFromSQL.ProductColors.Where(x => !productVM.ColorIds.Exists(xr => xr == x.ColorId)));
            _context.ProductColors.AddRange(productVM.ColorIds.Where(x => !productFromSQL.ProductColors.Exists(xd => xd.ColorId == x)).ToList().Select(xz => new ProductColor { ColorId = xz, ProductId = productFromSQL.Id }));


            _context.ProductTags.RemoveRange(productFromSQL.ProductTags.Where(x => !productVM.TagIds.Exists(xr => xr == x.TagId)));
            //List<int> addTags = productVM.TagIds.Where(x => !productFromSQL.ProductTags.Exists(xd => xd.TagId == x)).ToList();
            _context.ProductTags.AddRange(productVM.TagIds.Where(x => !productFromSQL.ProductTags.Exists(xd => xd.TagId == x)).ToList().Select(xz => new ProductTag { TagId = xz, ProductId = productFromSQL.Id }));
            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

                ProductImage main = productFromSQL.ProductImages.FirstOrDefault(p => p.IsPrimary == true);
                main.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                productFromSQL.ProductImages.Remove(main);
                productFromSQL.ProductImages.Add(new ProductImage
                {
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    IsPrimary = true,
                    Image = fileName
                });
            }

            if (productVM.HoverPhoto is not null)
            {
                string fileName = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");

                ProductImage hover = productFromSQL.ProductImages.FirstOrDefault(p => p.IsPrimary == false);
                hover.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                productFromSQL.ProductImages.Remove(hover);
                productFromSQL.ProductImages.Add(new ProductImage
                {
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                    IsPrimary = false,
                    Image = fileName
                });
            }
            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            var deletedImages = productFromSQL.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();

            deletedImages.ForEach(di => di.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images"));


            _context.ProductImages.RemoveRange(deletedImages);

            if (productVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;
                foreach (IFormFile file in productVM.AdditionalPhotos)
                {
                    if (!file.IsFileTypeValid("image/"))
                    {
                        text += $"<p class=\"text-warning\">{file.FileName} type was not correct</p>";
                        continue;
                    }
                    if (!file.IsFileSizeValid(FileSize.Megabyte, 1))
                    {
                        text += $"<p class=\"text-warning\">{file.FileName} size was not correct</p>";
                        continue;
                    }

                    productFromSQL.ProductImages.Add(new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        IsPrimary = null,

                    });
                }

                TempData["FileWarning"] = text;
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
