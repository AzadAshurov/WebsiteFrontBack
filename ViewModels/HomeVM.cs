using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class HomeVM
    {
        public List<Slide> Slides { get; set; } = new List<Slide>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Product> NewProducts { get; set; } = new List<Product>();

    }
}
