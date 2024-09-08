using System.Transactions;
using RazorPagesStock.Models;

namespace RazorPagesStock.DTOs
{
    public interface ITransaction
    {
        public bool success {get;set;}
        public IHolding holding{get;set;}
        public bool execute();
        
    }
}