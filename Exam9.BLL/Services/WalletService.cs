using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;

namespace Exam9.BLL.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<WalletAccount?> GetByUserIdAsync(int userId)
    {
        return await _walletRepository.GetByUserIdAsync(userId);
    }
    
    public async Task<bool> DepositAsync(string accountNumber, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 6 || !accountNumber.All(char.IsDigit))
        {
            throw new ArgumentException("Некорректный номер счёта.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Сумма должна быть больше нуля.");
        }

        return await _walletRepository.DepositAsync(accountNumber, amount);
    }
    
    public async Task<TransferResult> TransferAsync(int senderUserId, string receiverAccountNumber, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(receiverAccountNumber) || receiverAccountNumber.Length != 6 || !receiverAccountNumber.All(char.IsDigit))
        {
            throw new ArgumentException("Некорректный номер счёта получателя.");
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Сумма перевода должна быть больше нуля.");
        }

        var senderWallet = await _walletRepository.GetByUserIdAsync(senderUserId);

        if (senderWallet == null)
            return TransferResult.SenderNotFound;

        return await _walletRepository.TransferAsync(senderWallet.AccountNumber, receiverAccountNumber, amount);
    }
}