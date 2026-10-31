using Exam9.BLL.Interfaces;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;

namespace Exam9.BLL.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<List<Transaction>> GetByWalletIdAsync(int walletId, DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        if (walletId <= 0)
            throw new ArgumentException("Некорректный идентификатор кошелька.");

        if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
        {
            throw new ArgumentException("Начальная дата не может быть позже конечной.");
        }

        if (dateFrom.HasValue)
            dateFrom = DateTime.SpecifyKind(dateFrom.Value.Date, DateTimeKind.Utc);

        if (dateTo.HasValue)
            dateTo = DateTime.SpecifyKind(dateTo.Value.Date, DateTimeKind.Utc);

        return await _transactionRepository.GetByWalletIdAsync(walletId, dateFrom, dateTo);
    }
}