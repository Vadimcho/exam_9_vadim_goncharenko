using Exam9.DAL.Models;

namespace Exam9.BLL.Interfaces;

public interface ITransactionService
{
    Task<List<Transaction>> GetByWalletIdAsync(int walletId, DateTime? dateFrom = null, DateTime? dateTo = null);
}