using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Empty area")]
        [MaxLength(30, ErrorMessage = "Limit of length is 30")]
        public string Name { get; set; }
        // public DateTime CreatedAt { get; set; }

        //relation
        public List<Product>? Product { get; set; }
    }
}
