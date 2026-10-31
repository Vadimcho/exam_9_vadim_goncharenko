using Exam9.BLL.Interfaces;
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
}