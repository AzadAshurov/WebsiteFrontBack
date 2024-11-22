using Microsoft.AspNetCore.Mvc;
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

            //List<Slide> slides = new List<Slide>
            //{
            //    new Slide
            //    {

            //        Title = "Very interesting title",
            //        Description="Very cool, my uncle liked it",
            //        SubTitle = "Very interesting subtitle",
            //        Image = "e5f8d96c-3e16-4558-b633-c921c71228f8-1716396008555.jpg",
            //        Order = 1,
            //        IsDeleted = false,
            //        CreatedAt = DateTime.Now
            //    },
            //       new Slide
            //    {

            //        Title = "Very lorem ipsum dolor sit amet title",
            //        Description="Very cool, I liked it",
            //        SubTitle = "Very interesting subtitle",
            //        Image = "angry.png",
            //        Order = 3,
            //        IsDeleted = false,
            //        CreatedAt = DateTime.Now
            //    },
            //            new Slide
            //    {

            //        Title = "Very interesting title",
            //        Description="Very cool, all mexican cartel liked it",
            //        SubTitle = "Very interesting subtitle",
            //        Image = "Demonicicon.png",
            //        Order = 2,
            //        IsDeleted = false,
            //        CreatedAt = DateTime.Now
            //    }
            //};
            //_context.Slides.AddRange(slides);
            //_context.SaveChanges();

            HomeVM homeVM = new HomeVM
            {
                //i want all 3 slides to be on
                Slides = _context.Slides.OrderBy(x => x.Order).Take(3).ToList(),
                Products = _context.Products.Include(p => p.ProductImages).ToList()
            };
            return View(homeVM);
        }
    }
}
