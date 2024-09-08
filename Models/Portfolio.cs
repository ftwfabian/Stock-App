using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesStock.Models
{
	public class Portfolio
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double CashBalance { get; set; } = 0;
		public string? UserId { get; set; }



	}
}
