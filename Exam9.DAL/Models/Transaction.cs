using Exam9.DAL.Enums;

namespace Exam9.DAL.Models;

public class Transaction
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? SenderWalletId { get; set; }
    public WalletAccount? SenderWallet { get; set; }

    public int? ReceiverWalletId { get; set; }
    public WalletAccount? ReceiverWallet { get; set; }

    public int? ProviderAccountId { get; set; }
    public ProviderAccount? ProviderAccount { get; set; }
}