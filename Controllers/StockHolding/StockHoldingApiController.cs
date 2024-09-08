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

namespace RazorPagesStock.Controllers.StockHolding
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockHoldingApiController : ControllerBase
    {
        int quantity = 10;

        private readonly RazorPagesStock.Data.ApplicationDbContext _context;
        private readonly IStockService _stockHoldingService;
        private readonly IPortfolioService _portfolioService;

        private readonly UserManager<ApplicationUser> _userManager;

        public StockHoldingApiController(ApplicationDbContext context, IStockService stockHoldingService, IPortfolioService portfolioService,
             UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _stockHoldingService = stockHoldingService;
            _portfolioService = portfolioService;
            _userManager = userManager;

        }
        
        [HttpPost]
        //public async Task<IActionResult> BuyShares(string ticker, int quantity, int portfolioId)
        public async Task<IActionResult> BuyShares([FromBody] ShareTransactionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new {message = "User Not Found"});
            }

            var response = await _portfolioService.BuyShares(request.Ticker, request.Quantity, request.PortfolioId, user.Id);
                
            if(response.success)
            {
                return Ok(new {message = "Trade executed successfully"});
            }
            else if (response.cashShortage)
            {
                return BadRequest(new {message = "Not enough cash to make the trade"});
            }
            else
            {
                //the only reason I need this is in case I want to get more descriptive to the front-end about the type of error
                return BadRequest(new {message = "Trade failed"});
            }
        }

        [HttpPost]
        public async Task<IActionResult> SellShares([FromBody] ShareTransactionRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new {message = "User Not Found"});
            }

            var response = await _portfolioService.SellShares(request.Ticker, request.Quantity, request.PortfolioId, user.Id);
                
            if(response.success)
            {
                return Ok(new {message = "Trade executed successfully"});
            }
            else if (response.shareShortage)
            {
                return BadRequest(new {message = "Not enough shares to make the trade"});
            }
            else
            {
                //the only reason I need this is in case I want to get more descriptive to the front-end about the type of error
                return BadRequest(new {message = "Trade failed"});
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentQuantitySharesHeld(string ticker, int portfolioId)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound(new {message="User Not Found"});
            }

            var response = await _portfolioService.GetCurrentQuantitySharesHeld(ticker, portfolioId, user.Id);

            return Ok(new {quantitySharesHeld = response});
        }


        public class ShareTransactionRequest
        {
            public required string Ticker {get;set;}
            public required int Quantity {get;set;}
            public required int PortfolioId {get;set;}  

        }


        
    }
}
