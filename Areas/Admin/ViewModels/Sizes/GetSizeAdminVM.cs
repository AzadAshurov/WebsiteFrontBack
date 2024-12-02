using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.ViewModels.Sizes
{
    public class GetSizeAdminVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
