namespace RazorPagesStock.Models
{
    public class FavoriteStock
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Ticker { get; set; }

        public ApplicationUser User { get; set; }
        public Stock Stock  { get; set; }

       
    }
}
