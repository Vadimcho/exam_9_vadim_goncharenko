using Exam9.DAL.Data;
using Exam9.DAL.Enums;
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
    
    public async Task<bool> DepositAsync(string accountNumber, decimal amount)
    {
        await using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var wallet = await _context.WalletAccounts
                .FirstOrDefaultAsync(w => w.AccountNumber == accountNumber);

            if (wallet == null)
                return false;

            wallet.Balance += amount;

            var transaction = new Transaction
            {
                Amount = amount,
                Type = TransactionType.Deposit,
                CreatedAt = DateTime.UtcNow,
                ReceiverWalletId = wallet.Id
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            await dbTransaction.CommitAsync();

            return true;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<TransferResult> TransferAsync(string senderAccountNumber, string receiverAccountNumber, decimal amount)
    {
        await using var dbTransaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try
        {
            if (senderAccountNumber == receiverAccountNumber)
            {
                await dbTransaction.RollbackAsync();
                return TransferResult.SameAccount;
            }

            var sender = await _context.WalletAccounts
                .FirstOrDefaultAsync(w => w.AccountNumber == senderAccountNumber);

            if (sender == null)
            {
                await dbTransaction.RollbackAsync();
                return TransferResult.SenderNotFound;
            }
            
            var receiver = await _context.WalletAccounts
                .FirstOrDefaultAsync(w => w.AccountNumber == receiverAccountNumber);

            if (receiver == null)
            {
                await dbTransaction.RollbackAsync();
                return TransferResult.ReceiverNotFound;
            }

            if (sender.Balance < amount)
            {
                await dbTransaction.RollbackAsync();
                return TransferResult.InsufficientFunds;
            }
            
            sender.Balance -= amount;
            receiver.Balance += amount;

            var transaction = new Transaction
            {
                Amount = amount,
                Type = TransactionType.Transfer,
                CreatedAt = DateTime.UtcNow,
                SenderWalletId = sender.Id,
                ReceiverWalletId = receiver.Id
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            await dbTransaction.CommitAsync();

            return TransferResult.Success;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}