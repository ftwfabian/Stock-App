namespace RazorPagesStock.Models
{
    public interface IHolding
    {
        public int Id { get; set; }
		public int Quantity { get; set; }
		public int PortfolioId { get; set; }
        public string UserId { get; set; }
		public DateTime PurchaseDate { get; set; }
    }
}