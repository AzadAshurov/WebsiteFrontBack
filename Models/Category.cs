namespace WebApplication1.Models
{
	public class Category : BaseEntity
	{

		public string Name { get; set; }
		//relation
		public List<Product> Product { get; set; }
	}
}
