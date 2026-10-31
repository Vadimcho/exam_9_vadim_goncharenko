using Exam9.DAL.Data;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Exam9.DAL.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WalletAccount?> GetByIdAsync(int id)
    {
        return await _context.WalletAccounts.FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WalletAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.WalletAccounts.FirstOrDefaultAsync(w => w.AccountNumber == accountNumber);
    }

    public async Task<WalletAccount?> GetByUserIdAsync(int userId)
    {
        return await _context.WalletAccounts.FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task AddAsync(WalletAccount wallet)
    {
        await _context.WalletAccounts.AddAsync(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WalletAccount wallet)
    {
        _context.WalletAccounts.Update(wallet);
        await _context.SaveChangesAsync();
    }
}