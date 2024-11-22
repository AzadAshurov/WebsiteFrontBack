namespace WebApplication1.Models
{
	public class Product : BaseEntity
	{
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Description { get; set; }
		public string SKU { get; set; }

		// relation
		public int CategoryId { get; set; }
		public Category Category { get; set; }
	}
}
