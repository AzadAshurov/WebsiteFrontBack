namespace WebApplication1.Areas.Admin.ViewModels.Categoryes
{
    public class CreateCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductCount { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
