using Exam9.DAL.Enums;
using Exam9.DAL.Models;

namespace Exam9.DAL.Interfaces;

public interface IWalletRepository
{
    Task<WalletAccount?> GetByIdAsync(int id);
    Task<WalletAccount?> GetByAccountNumberAsync(string accountNumber);
    Task<WalletAccount?> GetByUserIdAsync(int userId);
    Task AddAsync(WalletAccount wallet);
    Task UpdateAsync(WalletAccount wallet);
    Task<bool> DepositAsync(string accountNumber, decimal amount);
    Task<TransferResult> TransferAsync(string senderAccountNumber, string receiverAccountNumber, decimal amount);
}