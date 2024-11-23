﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var slides = _context.Slides.OrderBy(x => x.Order).Take(3).ToList();
            var products = _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToList();

            Console.WriteLine($"Slides count: {slides.Count}");
            Console.WriteLine($"Products count: {products.Count}");

            HomeVM homeVM = new HomeVM
            {
                Slides = slides,
                Products = products
            };
            return View(homeVM);
        }

    }
}
