using Pustok.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string? AppUserId { get;set; }
		[MaxLength(35)]
		public string FullName { get; set; }
		[MaxLength(100)]
		public string Email { get; set; }
		[MaxLength(15)]
		public string Phone { get; set; }
		[MaxLength(150)]
		public string Address { get; set; }
		public decimal TotalAmount { get; set; }
		[MaxLength(250)]
		public string Note { get; set; }
		public DateTime CreatedAt { get; set; }
		public OrderStatus Status { get; set; }

		public AppUser AppUser { get; set; }	
		public List<OrderItem> OrderItems { get; set;}
	}
}
