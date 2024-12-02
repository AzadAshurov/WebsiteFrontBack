using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Areas.Admin.ViewModels.Sizes
{
    public class UpdateSizeVM
    {
        [MaxLength(20)]
        public string Name { get; set; }
        public int Id { get; set; }
        public List<int> SizeIds { get; set; } = new List<int>();
    }
}
