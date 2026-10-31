namespace Exam9.DAL.Models;

public class ProviderAccount
{
    public int Id { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public int ServiceProviderId { get; set; }

    public ServiceProvider ServiceProvider { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}