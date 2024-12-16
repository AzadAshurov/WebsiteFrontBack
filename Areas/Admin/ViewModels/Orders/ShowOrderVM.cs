using WebApplication1.Areas.Admin.Models;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.ViewModels.Orders
{
    public class ShowOrderVM
    {
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public bool? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
