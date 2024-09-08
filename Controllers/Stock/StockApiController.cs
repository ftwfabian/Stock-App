using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Areas.Services;
using RazorPagesStock.Data;
using RazorPagesStock.Models;
using Microsoft.AspNetCore.Identity;
using SQLitePCL;
using System.Security.Claims;

namespace RazorPagesStock.Controllers.Portfolio
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockApiController : ControllerBase
    {
        int quantity = 10;

        private readonly RazorPagesStock.Data.ApplicationDbContext _context;
        private readonly IStockService _stockService;
        private readonly IPortfolioService _portfolioService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StockApiController(ApplicationDbContext context, IStockService stockService, IPortfolioService portfolioService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _stockService = stockService;
            _portfolioService = portfolioService;
            _userManager = userManager;

        }
        
        [HttpGet]
        public async Task<JsonResult> GetListedTickers()
        {
            //Right now this just grabs a single one, must be changed before it works as intended
            
            var tickers = await _context.Stocks
                .Select(t=>t.Ticker)
                .Take(quantity)
                .ToListAsync();
            
            return new JsonResult(tickers);
            
        } 
        
        [HttpGet]
        public async Task<JsonResult> GetStockData(string ticker)
        {

            var stockPrices = await _context.StockPrice
                .Where(v => v.Ticker == ticker)
                .Select(v => v.Price)
                .ToListAsync();

            var stockDates = await _context.StockPrice
                .Where(v => v.Ticker == ticker)
                .Select(v => v.Date)
                .ToListAsync();

            
            var response = new {
                Dates = stockDates,
                Prices = stockPrices
            };
            
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetStockDataWithTimePeriod(string ticker, int timePeriod)
        {
            DateTime latestDate = await _context.StockPrice
                .Where(v => v.Ticker == ticker)
                .MaxAsync(v => v.Date);
            //timePeriod should always be expressed in days
            var stockData = await _context.StockPrice
                .Where(v => v.Ticker == ticker && v.Date > latestDate.AddDays(-timePeriod))
                .OrderBy(v=> v.Date)
                .Select(v => new {v.Price,v.Date})
                .ToListAsync();

            var response = new 
            {
                Dates = stockData.Select(sp=> sp.Date).ToList(),
                Prices = stockData.Select(sp => sp.Price).ToList()    
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<Boolean>> TickerExistsCheck(string ticker)
        {
            bool exists = await _context.Stocks.AnyAsync(t => t.Ticker == ticker);
            return Ok(exists);
        }

        [HttpGet]
        public async Task<ActionResult<long>> GetMarketCap(string ticker)
        {
            return await _stockService.GetMarketCap(ticker);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetSortedTickers(string sortCriteria,
            string sortOrder){

            var sortedTickers = await _stockService.GetSortedTickersAsync(sortCriteria,
                sortOrder, quantity);
            
            return new JsonResult(sortedTickers);
        }



        
    }
}
