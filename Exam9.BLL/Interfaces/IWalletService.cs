using Exam9.DAL.Models;

namespace Exam9.BLL.Interfaces;

public interface IWalletService
{
    Task<WalletAccount?> GetByUserIdAsync(int userId);
}