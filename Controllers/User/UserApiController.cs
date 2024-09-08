using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Areas.Services;
using RazorPagesStock.Data;
using RazorPagesStock.Models;
using SQLitePCL;
using System.Security.Claims;

namespace RazorPagesStock.Controllers.User
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        int quantity = 10;

        private readonly RazorPagesStock.Data.ApplicationDbContext _context;
        private readonly IStockService _stockService;
        private readonly IPortfolioService _portfolioService;
        private readonly UserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserApiController(ApplicationDbContext context, IStockService stockService, IPortfolioService portfolioService,
            UserService userService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _stockService = stockService;
            _portfolioService = portfolioService;
            _userService = userService;
            _userManager = userManager;

        }
        
        [HttpPost]
        public async Task<ActionResult<object>> AddFavoriteStock(string ticker)
        {
            //I'm not really certain that this messaging is all that necessary
            //because of how low stakes the Favoriting of stocks is.

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new {message = "User Not Found"});
            }

            var result = await _userService.AddFavoriteStockAsync(ticker, user.Id);

            if(result.Success)
            {
                return Ok(new { tickerIsFavorite = "Favorite Added Successfully" });
            }
            else
            {
                return BadRequest(new { message = result.Message});
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteStockBoolean(string ticker)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound(new {message = "User Not Found"});
            }

            var result = await _userService.GetFavoriteStockBooleanAsync(ticker,user.Id);

            if(result.Success)
            {
                return Ok(new {tickerIsFavorite = result.TickerIsFavorite});
            }

            return BadRequest(new {message = result.Message});
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFavoriteStock(string ticker)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new {message = "User Not Found"});
            }

            var result = await _userService.DeleteFavoriteStockAsync(ticker, user.Id);

            if(result.Success)
            {
                return Ok(new {message = "Stock Successfully Deleted"});
            }

            return BadRequest(new {message = result.Message});
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPortfolios()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound(new {message = "User Not Found"});
            }

            var result = await _userService.GetUserPortfoliosAsync(user.Id);
            var trimmedResult = result.Select(x => (new {x.Name, x.Id}));

            return Ok(new {message = "Found portfolios", result = trimmedResult});
        }

        
    }

    public class PurchaseSharesRequest
    {
        public string? Ticker { get; set; }
        public int Quantity { get; set; }
        public int PortfolioId { get; set; }
    }
}
