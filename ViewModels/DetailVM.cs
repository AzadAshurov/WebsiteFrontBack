
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class DetailVM
    {
        public Product Products { get; set; }
        public List<Product> RelatedProducts { get; set; } = new List<Product>();

    }
}

