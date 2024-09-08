using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Data;

namespace RazorPagesStock.Areas.Services
{
	public class StockService:IStockService
	{
		private readonly RazorPagesStock.Data.ApplicationDbContext _context;

		public StockService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<double> GetCurrentPriceAsync(string ticker)
        {
            var price = await _context.StockPrice
                .Where(x => x.Ticker == ticker)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Price)
                .FirstOrDefaultAsync();

            return price;
        }

		public async Task<List<RazorPagesStock.Models.StockPrice>> GetStockValuesOverTime(string ticker, int days)
		{
			var maxDate = await _context.StockPrice
				.Where(h => h.Ticker == ticker)
				.MaxAsync(h => h.Date);

			var stockPriceSeries = await _context.StockPrice
				.Where(h => h.Ticker == ticker && h.Date > maxDate.AddDays(-days))
				.GroupBy(h => h.Date)
				.Select(g => g.OrderByDescending(h => h.Date).FirstOrDefault())
				.ToListAsync();

			return stockPriceSeries ?? new List<RazorPagesStock.Models.StockPrice>();
		}

		public async Task<long> GetMarketCap(string ticker)
		{
			var currentPrice = await _context.StockPrice
    			.Where(h => h.Ticker == ticker)
    			.OrderByDescending(h => h.Date)
    			.FirstOrDefaultAsync();

			/**
			var sharesOutstanding = await _context.Stocks
				.Where(h => h.Ticker == ticker)
				.Select(h => h.SharesOutstanding)
				.FirstOrDefaultAsync();
				**/
			if(currentPrice != null){
				return (long)currentPrice.Price * currentPrice.SharesOutstanding;
			}

			return 0;
			
		}
		

		public IEnumerable<DateTimeOffset> GetRecentDates(int howRecent)
		{
			List<DateTimeOffset> dates = new List<DateTimeOffset>();
			int start = (howRecent - 1) * -1;
			for (int i = 0; i < howRecent; i++)
			{
				dates.Add(new DateTimeOffset(DateTime.Now.AddDays(start).Date));
				start++;
			}
			return dates;
		}

		public async Task<List<string>> GetSortedTickersAsync(string sortCriteria, string sortOrder, int quantity)
		{
			
			
			List<(string ticker,long marketCap)> tickerAndMarketCap = new List<(string, long)>();
			List<string> result = new List<string>();

			var tickers = await _context.Stocks
				.Select(t=>t.Ticker)
				.ToListAsync();

			var queryStock = _context.Stocks.AsQueryable();
			var queryStockPrice = _context.Stocks.AsQueryable();

			if(sortCriteria == "marketcap"){
				foreach(var ticker in tickers )
				{
					tickerAndMarketCap.Add((ticker, await GetMarketCap(ticker)));
				}
				if(sortOrder == "asc"){
					result = tickerAndMarketCap
						.OrderBy(t=>t.marketCap)
						.Select(t=>t.ticker)
						.Take(quantity)
						.ToList();
				}
				else
				{
					//just a reminder that you are likely only ever going to want desc
					result = tickerAndMarketCap
						.OrderByDescending(t=>t.marketCap)
						.Select(t=>t.ticker)
						.Take(quantity)
						.ToList();
				}
			}

			return result;

		}

	}
}
