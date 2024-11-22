namespace WebApplication1.Models
{
	public class Category : BaseEntity
	{

		public string Name { get; set; }
		//relation
		public Product Product { get; set; }
	}
}
