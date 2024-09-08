using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RazorPagesStock.Areas.Services;
using RazorPagesStock.Data;
using System.Security.Claims;

namespace RazorPagesStock.Controllers.Portfolio
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PortfolioApiController : ControllerBase
    {

        private readonly RazorPagesStock.Data.ApplicationDbContext _context;
        private readonly IStockService _stockService;
        private readonly IPortfolioService _portfolioService;

        public PortfolioApiController(ApplicationDbContext context, IStockService stockService, IPortfolioService portfolioService)
        {
            _context = context;
            _stockService = stockService;
            _portfolioService = portfolioService;

        }

        [HttpGet]
        public async Task<JsonResult> GetPortfolioHoldings(int portfolioId) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var portfolio = _context.Portfolios.FirstOrDefault(t => t.UserId == userId && t.Id == portfolioId);

            if (portfolio == null)
            {
                return new JsonResult(new { success = false, message = "Unauthorized" }) { StatusCode = 401 };
            }

            var holdings = await _portfolioService.GetPortfolioHoldings(portfolioId);

            var response = new
            {

                PortfolioName = portfolio.Name,
                Holdings = holdings
                
            };
            return new JsonResult(response);

        }

        [HttpGet]
        public async Task<JsonResult> GetStockCanvasData(string ticker, int days)
        {
            var tickerPriceSeries = await _stockService.GetStockValuesOverTime(ticker, days);

            var response = new
            {
                Dates = tickerPriceSeries.Select(sp => sp.Date),
                Prices = tickerPriceSeries.Select(sp => sp.Price)
            };

            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<JsonResult> GetPortfolioCanvasData(int portfolioId, int days)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var portfolio = _context.Portfolios.FirstOrDefault(t => t.UserId == userId && t.Id == portfolioId);

            if (portfolio == null)
            {
                return new JsonResult(new { success = false, message = "Unauthorized" }) { StatusCode = 401 };
            }

            var portfolioName = portfolio.Name;
            var portfolioValues = await _portfolioService.GetPortfolioDailyValue(portfolio, days);
            var dates = portfolioValues.Select(pv => pv.TimeStamp).ToArray();
            var prices = portfolioValues.Select(pv => pv.Price).ToArray();

            var response = new
            {
                PortfolioName = portfolioName,
                Dates = dates,
                Prices = prices
            };

            return new JsonResult(response);


        }
    }
}
