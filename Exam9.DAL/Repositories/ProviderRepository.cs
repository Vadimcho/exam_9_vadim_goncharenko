using Exam9.DAL.Data;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Exam9.DAL.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly ApplicationDbContext _context;

    public ProviderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceProvider>> GetAllAsync()
    {
        return await _context.ServiceProviders.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<ServiceProvider?> GetByIdAsync(int id)
    {
        return await _context.ServiceProviders.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProviderAccount?> GetProviderAccountAsync(int providerId, string accountNumber)
    {
        return await _context.ProviderAccounts
            .FirstOrDefaultAsync(a => a.ServiceProviderId == providerId && a.AccountNumber == accountNumber);
    }

    public async Task UpdateAccountAsync(ProviderAccount account)
    {
        _context.ProviderAccounts.Update(account);
        await _context.SaveChangesAsync();
    }
}