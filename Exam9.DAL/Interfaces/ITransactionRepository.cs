using Exam9.DAL.Models;

namespace Exam9.DAL.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<List<Transaction>> GetByWalletIdAsync(int walletId, DateTime? dateFrom = null, DateTime? dateTo = null);
}