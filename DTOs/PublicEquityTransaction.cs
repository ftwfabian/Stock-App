using Microsoft.EntityFrameworkCore;
using RazorPagesStock.Data;
using RazorPagesStock.Models;

namespace RazorPagesStock.DTOs
{
    public class PublicEquityTransaction : ITransaction
    {
        public bool success { get; set; }
        public IHolding holding { get; set; }
        private readonly ApplicationDbContext _context;

        public PublicEquityTransaction(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public bool execute()
        {
            return true;
        }
        

    }
}