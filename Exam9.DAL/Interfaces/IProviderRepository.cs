using Exam9.DAL.Models;

namespace Exam9.DAL.Interfaces;

public interface IProviderRepository
{
    Task<List<ServiceProvider>> GetAllAsync();
    Task<ServiceProvider?> GetByIdAsync(int id);
    Task<ProviderAccount?> GetProviderAccountAsync(int providerId, string accountNumber);
    Task UpdateAccountAsync(ProviderAccount account);
}