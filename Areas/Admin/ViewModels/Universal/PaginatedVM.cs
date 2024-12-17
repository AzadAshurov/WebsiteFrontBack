namespace WebApplication1.Areas.Admin.ViewModels.Universal
{
    public class PaginatedVM<T>
    {
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<T> Items { get; set; }
    }
}
