using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RazorPagesStock.Models;


namespace RazorPagesStock.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Stock>()
                .HasKey(s => new{s.Id,s.Ticker});

            modelBuilder.Entity<FavoriteStock>()
                .HasKey(fs => fs.Id);

            modelBuilder.Entity<FavoriteStock>()
                .HasOne(fs => fs.User)
                .WithMany(u => u.FavoriteStocks)
                .HasForeignKey(fs => fs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteStock>()
                .HasOne(fs => fs.Stock)
                .WithMany()
                .HasForeignKey(fs => fs.Ticker)
                .HasPrincipalKey(s => s.Ticker)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
        public DbSet<RazorPagesStock.Models.ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<RazorPagesStock.Models.Portfolio> Portfolios { get; set; }
        public DbSet<RazorPagesStock.Models.FavoriteStock> FavoriteStocks { get; set; }
        public DbSet<RazorPagesStock.Models.Stock> Stocks { get; set; }
        public DbSet<RazorPagesStock.Models.StockPrice> StockPrice { get; set; }
        public DbSet<RazorPagesStock.Models.StockHolding> StockHolding { get; set; } = default!;


	

	}
}
