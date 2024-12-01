namespace WebApplication1.Models
{
    public class Tag : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ProductTag> ProductTags { get; set; }


    }
}
