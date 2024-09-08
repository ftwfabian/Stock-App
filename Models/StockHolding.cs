using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesStock.Models
{
	public class StockHolding:IHolding
	{
		public int Id { get; set; }

		public required string Ticker { get; set; }

		[Range(1,int.MaxValue)]
		public int Quantity { get; set; }

		public int PortfolioId { get; set; }

        public string UserId { get; set; }

		public DateTime PurchaseDate { get; set; }
    }
}
