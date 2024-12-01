using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Areas.Admin.ViewModels.Tags
{
    public class UpdateTagVM
    {
        [MaxLength(20)]
        public string Name { get; set; }
        public int Id { get; set; }

    }
}
