using Exam9.DAL.Data;
using Exam9.DAL.Enums;
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
    
    public async Task<PaymentResult> PayAsync(int walletId, int providerId, string providerAccountNumber, decimal amount)
    {
        await using var dbTransaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try
        {
            var wallet = await _context.WalletAccounts.FirstOrDefaultAsync(w => w.Id == walletId);
            
            if (wallet == null)
            {
                await dbTransaction.RollbackAsync();
                return PaymentResult.WalletNotFound;
            }

            var providerExists = await _context.ServiceProviders.AnyAsync(p => p.Id == providerId);

            if (!providerExists)
            {
                await dbTransaction.RollbackAsync();
                return PaymentResult.ProviderNotFound;
            }

            var providerAccount = await _context.ProviderAccounts
                .FirstOrDefaultAsync(a => a.ServiceProviderId == providerId && a.AccountNumber == providerAccountNumber);

            if (providerAccount == null)
            {
                await dbTransaction.RollbackAsync();
                return PaymentResult.AccountNotFound;
            }

            if (wallet.Balance < amount)
            {
                await dbTransaction.RollbackAsync();
                return PaymentResult.InsufficientFunds;
            }

            wallet.Balance -= amount;
            providerAccount.Balance += amount;

            var transaction = new Transaction
            {
                Amount = amount,
                Type = TransactionType.Payment,
                CreatedAt = DateTime.UtcNow,
                SenderWalletId = wallet.Id,
                ProviderAccountId = providerAccount.Id
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            await dbTransaction.CommitAsync();

            return PaymentResult.Success;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}