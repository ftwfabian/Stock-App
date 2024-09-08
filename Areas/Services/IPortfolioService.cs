using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Data;

namespace RazorPagesStock.Areas.Services
{
    public interface IPortfolioService
    {

        Task<List<(double Price, DateTime TimeStamp)>> GetPortfolioDailyValue(RazorPagesStock.Models.Portfolio selectedPortfolio, int days);
        Task<List<RazorPagesStock.Models.StockHolding>> GetPortfolioHoldings(int portfolioId);
        public Task<(Boolean success, Boolean cashShortage, Boolean otherFailure)> BuyShares(string ticker, int quantity, int portfolioId, string userId);
        public Task<int> GetCurrentQuantitySharesHeld(string ticker, int portfolioId, string userId);
        public Task<(Boolean success, Boolean shareShortage, Boolean otherFailure)> SellShares(string ticker, int quantity, int portfolioId, string userId);
        
    }
}
