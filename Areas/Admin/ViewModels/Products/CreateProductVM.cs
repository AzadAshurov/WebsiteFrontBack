using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.ViewModels.Products
{
    public class CreateProductVM
    {
        public IFormFile MainPhoto { get; set; }

        public IFormFile HoverPhoto { get; set; }

        public List<IFormFile>? SidePhotos { get; set; }
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }

        // relation
        [Required(ErrorMessage = "PLEASE SELECT CATEGORY")]
        public int? CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<int>? TagIds { get; set; }
        public List<Color>? Colors { get; set; }
        public List<int>? ColorIds { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<int>? SizeIds { get; set; }
    }
}
