using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
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
