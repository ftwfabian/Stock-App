using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Data;
using RazorPagesStock.Models;

namespace RazorPagesStock.Areas.Services
{
	public class PortfolioService: IPortfolioService
	{
		private readonly RazorPagesStock.Data.ApplicationDbContext _context;
		private readonly IStockService _stockPriceService;

		public PortfolioService(ApplicationDbContext context, IStockService stockPriceService)
		{
			_context = context;
			_stockPriceService = stockPriceService;
		}

		public async Task<List<(double Price,DateTime TimeStamp)>> GetPortfolioDailyValue(RazorPagesStock.Models.Portfolio selectedPortfolio, int days)
		{
			var portfolioValues = new List<(double Price, DateTime TimeStamp)>();

			var holdings = await _context.StockHolding
				.Where(h => h.PortfolioId == selectedPortfolio.Id)
				.ToListAsync();

			//var endDate = DateTime.UtcNow.Date;
			
			//Some jury rigging is in order...
			var firstTicker = holdings.FirstOrDefault()?.Ticker;
			var endDate = await _context.StockPrice
				.Where(h => h.Ticker == firstTicker)
				.MaxAsync(sp => sp.Date);
			//...Because of the way I have created my test data, where it extends into the future

			//var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days + 1);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
			{
				double portfolioValue = 0;

				foreach(var holding in holdings)
				{
					var stockValue = await GetStockValueOnDate(holding.Ticker, date);
					portfolioValue += stockValue * holding.Quantity;
				}
				portfolioValues.Add((Price: portfolioValue, TimeStamp: date));
			}

			return portfolioValues;
			
		}

		public async Task<List<RazorPagesStock.Models.StockHolding>> GetPortfolioHoldings(int portfolioId)
		{
			var holdings = await _context.StockHolding
				.Where(h => h.PortfolioId == portfolioId)
				.GroupBy(h => h.Ticker)
				.Select(g => new RazorPagesStock.Models.StockHolding
				{
					Ticker = g.Key,
					Quantity = g.Sum(h => h.Quantity)
				})
				.ToListAsync();

			return holdings;
		}

		private async Task<double> GetStockValueOnDate(string ticker, DateTime date) //Remember that this is private, so do not remove
		{
			// Retrieve the stock value for the specified ticker and date from your data source
			// You can use an API, database, or any other means to get the historical stock data
			// For simplicity, let's assume you have a method that fetches the stock value for a given date

			var stockValue = await _context.StockPrice.FirstOrDefaultAsync(t => t.Ticker == ticker && t.Date == date);
			if (stockValue == null)
			{
				return 0;
			}
			return stockValue.Price;
		}

        public async Task<(Boolean success, Boolean cashShortage, Boolean otherFailure)> BuyShares(string ticker, int quantity, int portfolioId, string userId)
        {

            var existingHolding = _context.StockHolding
                .FirstOrDefault(p => p.Ticker == ticker && p.PortfolioId == portfolioId
                    && p.UserId == userId);

            var price = await _stockPriceService.GetCurrentPriceAsync(ticker);

            var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId);


            if(portfolio != null && portfolio.CashBalance - quantity * price < 0)
            {
                return (false, true, false);
            }


            if(existingHolding != null && portfolio != null && portfolio.CashBalance - quantity * price >= 0)
            {
                try
                {
                    existingHolding.Quantity += quantity;
                    portfolio.CashBalance -= quantity * price;
                    await _context.SaveChangesAsync();
                    return (true, false, false);
                }
                catch(Exception)
                {
                    return (false, false, true);
                }
            } 
            else if (existingHolding == null && portfolio != null && portfolio.CashBalance - quantity * price >= 0)
            {
                try
                {
                    RazorPagesStock.Models.StockHolding stockHolding = new RazorPagesStock.Models.StockHolding
                    {
                        Ticker = ticker,
                        Quantity = quantity,
                        UserId = userId,
                        PortfolioId = portfolioId,
                        PurchaseDate = DateTime.UtcNow
                    };

                    _context.StockHolding.Add(stockHolding);

                    await _context.SaveChangesAsync();
                    
                    return (true, false, false);

                } 
                catch (Exception)
                {
                    return (false, false, true);
                }

            } 
            else 
            {
                return (false, false, true);
            }

        }
        public async Task<int> GetCurrentQuantitySharesHeld(string ticker, int portfolioId, string userId)
        {
            var quantity = await _context.StockHolding
                .Where(x => x.Ticker == ticker && x.UserId == userId && x.PortfolioId == portfolioId)
                .Select(x => x.Quantity)
                .FirstOrDefaultAsync();

            return quantity;
        }

        public async Task<(Boolean success, Boolean shareShortage, Boolean otherFailure)> SellShares(string ticker, int quantity, int portfolioId, string userId)
        {

            var existingHolding = _context.StockHolding
                .FirstOrDefault(p => p.Ticker == ticker && p.PortfolioId == portfolioId
                    && p.UserId == userId);

            var price = await _stockPriceService.GetCurrentPriceAsync(ticker);

            var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId);


            if(portfolio != null && (existingHolding == null || existingHolding.Quantity - quantity < 0))
            {
                return (false, true, false);
            }


            if(existingHolding != null && portfolio != null && existingHolding.Quantity - quantity >= 0)
            {
                try
                {
                    existingHolding.Quantity -= quantity;
                    portfolio.CashBalance += quantity * price;
                    await _context.SaveChangesAsync();
                    return (true, false, false);
                }
                catch(Exception)
                {
                    return (false, false, true);
                }
            } 
            else 
            {
                return (false, false, true);
            }

        }


	}
}
