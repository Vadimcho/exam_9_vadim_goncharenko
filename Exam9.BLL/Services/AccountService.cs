using System.Security.Cryptography;
using Exam9.BLL.Interfaces;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;

namespace Exam9.BLL.Services;

public class AccountService : IAccountService
{
    private readonly IWalletRepository _walletRepository;

    private const decimal InitialBalance = 100000m;

    public AccountService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<string> GenerateAccountNumberAsync()
    {
        string accountNumber;

        do
        {
            accountNumber = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }
        while (await _walletRepository.GetByAccountNumberAsync(accountNumber) != null);

        return accountNumber;
    }

    public async Task CreateWalletAsync(int userId, string accountNumber)
    {
        var wallet = new WalletAccount
        {
            AccountNumber = accountNumber,
            Balance = InitialBalance,
            UserId = userId
        };

        await _walletRepository.AddAsync(wallet);
    }
}