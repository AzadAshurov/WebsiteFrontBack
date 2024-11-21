using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int id)
        {
            List<Slide> slides = new List<Slide>
            {
                new Slide
                {
                    Id = 1,
                    Title = "Very interesting title",
                    Description="Very cool, my uncle liked it",
                    SubTitle = "Very interesting subtitle",
                    Image = "e5f8d96c-3e16-4558-b633-c921c71228f8-1716396008555.jpg",
                    Order = 1
                },
                   new Slide
                {
                    Id = 2,
                    Title = "Very lorem ipsum dolor sit amet title",
                    Description="Very cool, I liked it",
                    SubTitle = "Very interesting subtitle",
                    Image = "angry.png",
                    Order = 3
                },
                        new Slide
                {
                    Id = 2,
                    Title = "Very interesting title",
                    Description="Very cool, all mexican cartel liked it",
                    SubTitle = "Very interesting subtitle",
                    Image = "Demonicicon.png",
                    Order = 2
                }
            };

            HomeVM homeVM = new HomeVM
            {
                Slides = slides
            };
            return View();
        }
    }
}
