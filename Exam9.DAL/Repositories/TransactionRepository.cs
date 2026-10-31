using Exam9.DAL.Data;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Exam9.DAL.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetByWalletIdAsync(int walletId, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var query = _context.Transactions
            .Include(t => t.SenderWallet)
            .Include(t => t.ReceiverWallet)
            .Include(t => t.ProviderAccount)
            .ThenInclude(p => p!.ServiceProvider)
            .Where(t => t.SenderWalletId == walletId || t.ReceiverWalletId == walletId);

        if (dateFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            var exclusiveEnd = dateTo.Value.Date.AddDays(1);
            query = query.Where(t => t.CreatedAt < exclusiveEnd);
        }

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }
}