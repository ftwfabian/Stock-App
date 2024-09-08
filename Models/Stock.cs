namespace RazorPagesStock.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public required string Ticker { get; set; }
        public long SharesOutstanding {get; set;}
    }
}
