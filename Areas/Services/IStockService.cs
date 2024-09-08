using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Data;

namespace RazorPagesStock.Areas.Services
{
	public interface IStockService
	{
		public Task<double> GetCurrentPriceAsync(string ticker);
        public Task<long> GetMarketCap(string ticker);
		public IEnumerable<DateTimeOffset> GetRecentDates(int howRecent);
		public Task<List<string>> GetSortedTickersAsync(string sortCriteria, string sortOrder, int quantity);
        public Task<List<RazorPagesStock.Models.StockPrice>> GetStockValuesOverTime(string ticker, int days);
	}
}
