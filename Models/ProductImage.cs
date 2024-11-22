namespace WebApplication1.Models
{
	public class ProductImage : BaseEntity
	{
		public string Image { get; set; }
		public bool? IsPrimary { get; set; }
		//relation
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}
