using Exam9.DAL.Enums;
using Exam9.DAL.Models;

namespace Exam9.DAL.Interfaces;

public interface IProviderRepository
{
    Task<List<ServiceProvider>> GetAllAsync();
    Task<ServiceProvider?> GetByIdAsync(int id);
    Task<ProviderAccount?> GetProviderAccountAsync(int providerId, string accountNumber);
    Task UpdateAccountAsync(ProviderAccount account);
    Task<PaymentResult> PayAsync(int walletId, int providerId, string providerAccountNumber, decimal amount);
}