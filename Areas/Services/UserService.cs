using RazorPagesStock.Data;
using RazorPagesStock.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RazorPagesStock.Areas.Services
{
	public class UserService
	{
		private readonly ApplicationDbContext _context;
		//make some methods for UserServices
		public UserService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<(bool Success, string Message, FavoriteStock? FavoriteStock)> AddFavoriteStockAsync(string ticker, string userId)
		{
			if(string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(userId))
			{
				return (false, "Ticker and UserId Required", null);
			}

			var existingFavorite = await _context.FavoriteStocks
				.FirstOrDefaultAsync(f => f.Ticker == ticker && f.UserId == userId);

			if(existingFavorite != null)
			{
				return (false, "This stock is already a favorite", null);
			}

			var favoriteStock = new FavoriteStock
			{
				Ticker = ticker,
				UserId = userId
			};

			_context.FavoriteStocks.Add(favoriteStock);

			try
			{
				await _context.SaveChangesAsync();
				return (true, "Favorite Added Successfully", favoriteStock);
			}
			catch (Exception)
			{
				return (false,"An error occurred while saving the favorite.", null);
			}

		}

		public async Task<List<Portfolio>> GetUserPortfoliosAsync(string userId)
		{
			var userPortfolios = await _context.Portfolios
				.Where(p => p.UserId == userId)
				.ToListAsync();

			return userPortfolios;
		}

		public async Task<(bool Success, bool TickerIsFavorite, string Message)> GetFavoriteStockBooleanAsync(string ticker, string userId)
		{
			if(string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(userId))
			{
				return (false, false, "Ticker and UserId Required");
			}

			var favorite = await _context.FavoriteStocks
				.FirstOrDefaultAsync(f => f.Ticker == ticker && f.UserId == userId);

			if (favorite != null)
			{
				return (true, true, "Favorite found");
			} else 
			{
				return (true, false, "Favorite not found");
			}
		}

		public async Task<(bool Success, string Message)> DeleteFavoriteStockAsync(string ticker, string userId)
		{
			if(string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(userId))
			{
				return(false, "Ticker and UserId Required");
			}

			var favoriteStock = await _context.FavoriteStocks
				.FirstOrDefaultAsync(f => f.Ticker == ticker && f.UserId == userId);

			if (favoriteStock == null)
			{
				return (false, "FavoriteStock not found when trying to delete it");
			}

			try
			{
				_context.FavoriteStocks.Remove(favoriteStock);
				await _context.SaveChangesAsync();
				return (true, "Favorite Stock successfully removed");
			}
			catch (Exception)
			{
				return (true, "An error occurred as database actions to remove the favorite stock were running");
			}

			
		}
	}
}
