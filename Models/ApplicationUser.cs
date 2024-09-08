using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace RazorPagesStock.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string[]? Preferences { get; set; }
        public ICollection<FavoriteStock> FavoriteStocks { get; set; }

	}

    

}
