using Exam9.DAL.Enums;
using Exam9.DAL.Models;

namespace Exam9.BLL.Interfaces;

public interface IWalletService
{
    Task<WalletAccount?> GetByUserIdAsync(int userId);
    Task<bool> DepositAsync(string accountNumber, decimal amount);
    Task<TransferResult> TransferAsync(int senderUserId, string receiverAccountNumber, decimal amount);
}