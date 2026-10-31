using Exam9.DAL.Enums;

namespace Exam9.DAL.Models;

public class ServiceProvider
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ServiceType ServiceType { get; set; }

    public ICollection<ProviderAccount> ProviderAccounts { get; set; } = new List<ProviderAccount>();
}