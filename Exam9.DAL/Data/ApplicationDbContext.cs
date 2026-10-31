using Exam9.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exam9.DAL.Data;

public class ApplicationDbContext: IdentityDbContext<User, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {}
    
    public DbSet<WalletAccount> WalletAccounts => Set<WalletAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();
    public DbSet<ProviderAccount> ProviderAccounts => Set<ProviderAccount>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasOne(u => u.WalletAccount)
            .WithOne(w => w.User)
            .HasForeignKey<WalletAccount>(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Transaction>()
            .HasOne(t => t.SenderWallet)
            .WithMany(w => w.SentTransactions)
            .HasForeignKey(t => t.SenderWalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Transaction>()
            .HasOne(t => t.ReceiverWallet)
            .WithMany(w => w.ReceivedTransactions)
            .HasForeignKey(t => t.ReceiverWalletId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Transaction>()
            .HasOne(t => t.ProviderAccount)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.ProviderAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<ProviderAccount>()
            .HasOne(p => p.ServiceProvider)
            .WithMany(s => s.ProviderAccounts)
            .HasForeignKey(p => p.ServiceProviderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<WalletAccount>()
            .HasIndex(w => w.AccountNumber)
            .IsUnique();

        builder.Entity<ProviderAccount>()
            .HasIndex(p => new { p.ServiceProviderId, p.AccountNumber })
            .IsUnique();
        
        builder.Entity<WalletAccount>()
            .Property(w => w.Balance)
            .HasPrecision(18, 2);

        builder.Entity<ProviderAccount>()
            .Property(p => p.Balance)
            .HasPrecision(18, 2);

        builder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);
        
        builder.Entity<WalletAccount>()
            .Property(w => w.AccountNumber)
            .IsRequired()
            .HasMaxLength(6);

        builder.Entity<ProviderAccount>()
            .Property(p => p.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Entity<ServiceProvider>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Entity<WalletAccount>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_WalletAccounts_Balance",
                "\"Balance\" >= 0"));

        builder.Entity<ProviderAccount>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_ProviderAccounts_Balance",
                "\"Balance\" >= 0"));

        builder.Entity<Transaction>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Transactions_Amount",
                "\"Amount\" > 0"));

        builder.Entity<WalletAccount>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_WalletAccounts_AccountNumber",
                "\"AccountNumber\" ~ '^[0-9]{6}$'"));
    }
}


