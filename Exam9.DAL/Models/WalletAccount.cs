namespace Exam9.DAL.Models;

public class WalletAccount
{
    public int Id { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();

    public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
}