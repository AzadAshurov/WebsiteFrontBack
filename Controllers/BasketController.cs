﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{

    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketCookieItemVM> cookiesVM;
            string cookie = Request.Cookies["basket"];

            List<BasketItemVM> basketVM = new();

            if (cookie == null)
            {
                return View(basketVM);
            }

            cookiesVM = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);

            foreach (BasketCookieItemVM item in cookiesVM)
            {
                Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                if (product != null)
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


            return View(basketVM);
        }


        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result) return NotFound();

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

            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["basket"]);
        }


    }
}
