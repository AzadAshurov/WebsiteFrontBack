using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.ViewModels.Colors
{
    public class GetColorAdminVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
