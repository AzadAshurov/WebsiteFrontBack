using WebApplication1.Areas.Admin.Models;

namespace WebApplication1.Models
{
    public class Order : BaseEntity
    {

        public decimal TotalPrice { get; set; }
        //relation
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public bool? Status { get; set; }


    }
}
