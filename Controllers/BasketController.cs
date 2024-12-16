using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{

    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBasketService _basketService;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager, IBasketService basketService)
        {
            _context = context;
            _userManager = userManager;
            _basketService = basketService;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _basketService.GetBasketAsync());
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

            return RedirectToAction(nameof(GetBasket));
        }

        public async Task<IActionResult> GetBasket()
        {
            return PartialView("BasketPartialView", await _basketService.GetBasketAsync());
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
        [HttpPost]
        public async Task<IActionResult> ChangeItemQuantity(int? id, string change)
        {


            int changeInt = Convert.ToInt32(change);
            if (id == null || id < 1) return BadRequest();
            if (changeInt != 1 && changeInt != -1) return BadRequest();


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
                    return NotFound();
                }
                else
                {
                    item.Count += changeInt;
                    if (item.Count == 0) item.Count++;

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
                        existed.Count += changeInt;
                        if (existed.Count == 0) existed.Count++;
                    }
                    else
                    {
                        return NotFound();
                    }

                }
                else
                {
                    return NotFound();
                }
                string json = JsonConvert.SerializeObject(basket);

                Response.Cookies.Append("basket", json);

            }

            return RedirectToAction("Index", "Basket");

        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout()
        {
            OrderVM orderVM = new()
            {
                BasketInOrderVM = await _context.BasketItems
                    .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(bi => new BasketInOrderVM
                    {
                        Count = bi.Count,
                        Name = bi.Product.Name,
                        Price = bi.Product.Price,
                        SubTotal = bi.Product.Price * bi.Count
                    })
                    .ToListAsync()
            };

            return View(orderVM);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            List<BasketItem> basketItems = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Include(bi => bi.Product)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                orderVM.BasketInOrderVM = basketItems.Select(bi => new BasketInOrderVM
                {
                    Count = bi.Count,
                    Name = bi.Product.Name,
                    Price = bi.Product.Price,
                    SubTotal = bi.Product.Price * bi.Count
                }).ToList();

                return View(orderVM);
            }

            Order order = new Order
            {
                Address = orderVM.Address,
                Status = null,
                AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                OrderItems = basketItems.Select(bi => new OrderItem
                {
                    AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Count = bi.Count,
                    Price = bi.Product.Price,
                    ProductId = bi.ProductId
                }).ToList(),
                TotalPrice = basketItems.Sum(bi => bi.Product.Price * bi.Count)
            };

            await _context.Orders.AddAsync(order);
            _context.BasketItems.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
