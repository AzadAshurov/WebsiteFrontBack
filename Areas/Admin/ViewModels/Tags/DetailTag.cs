using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.ViewModels.Tags
{
    public class DetailTag
    {


        [MaxLength(20)]
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
