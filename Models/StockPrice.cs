namespace RazorPagesStock.Models
{
    public class StockPrice
    {
        public int Id { get; set; }
        public required string Ticker { get; set; }
        public required double Price { get; set; }
        public required DateTime Date { get; set; }
        public long SharesOutstanding { get; set; }
    }
}
