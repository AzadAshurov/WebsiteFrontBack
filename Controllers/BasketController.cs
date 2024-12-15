using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{

    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketVM = new();

            if (User.Identity.IsAuthenticated)
            {




                basketVM = await _context.BasketItems
                    .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi => new BasketItemVM
                    {
                        Count = bi.Count,
                        Price = bi.Product.Price,
                        Image = bi.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                        Name = bi.Product.Name,
                        SubTotal = bi.Product.Price * bi.Count,
                        Id = bi.ProductId

                    })
                    .ToListAsync();

            }
            else
            {
                List<BasketCookieItemVM> cookiesVM;
                string cookie = Request.Cookies["basket"];


                if (cookie is null)
                {
                    return View(basketVM);
                }

                cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
                foreach (BasketCookieItemVM item in cookiesVM)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(p => p.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (product is not null)
                    {
                        basketVM.Add(new BasketItemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Image = product.ProductImages[0].Image,
                            Price = product.Price,
                            Count = item.Count,
                            SubTotal = item.Count * product.Price

                        });
                    }
                }
            }


            return View(basketVM);
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) return NotFound();


            if (User.Identity.IsAuthenticated)
            {



                AppUser? user = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (item is null)
                {
                    user.BasketItems.Add(new BasketItem
                    {
                        ProductId = id.Value,
                        Count = 1,
                    });
                }
                else
                {
                    item.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];
                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        existed.Count++;
                    }
                    else
                    {

                        basket.Add(new BasketCookieItemVM()
                        {
                            Id = id.Value,
                            Count = 1
                        });
                    }

                }
                else
                {
                    basket = new List<BasketCookieItemVM>();

                    basket.Add(new BasketCookieItemVM()
                    {
                        Id = id.Value,
                        Count = 1
                    });

                }
                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("basket", json);

            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["basket"]);
        }
        public async Task<IActionResult> DeleteItemFromBasket(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                AppUser? user = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));


                BasketItem item = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (item is not null)
                {
                    user.BasketItems.Remove(item);
                }
                else
                {
                    return NotFound();
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;

                string cookies = Request.Cookies["basket"];
                if (cookies != null)
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookies);

                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed != null)
                    {
                        basket.Remove(existed);
                    }
                    else
                    {
                        //throw new Exception("Non existed id deleted");
                        return NotFound();
                    }

                }
                else
                {
                    return BadRequest();
                }
                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("basket", json);
            }


            return RedirectToAction("Index", "Basket");
        }


    }
}
