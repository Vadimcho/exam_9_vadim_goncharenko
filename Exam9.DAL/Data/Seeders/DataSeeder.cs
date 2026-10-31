using Exam9.DAL.Data;
using Exam9.DAL.Enums;
using Exam9.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Exam9.DAL.Data.Seeders;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var providersData = new[]
        {
            new
            {
                Name = "Beeline",
                Type = ServiceType.MobileCommunications,
                Accounts = new[] { "0555123456", "0555987654" }
            },
            new
            {
                Name = "O!",
                Type = ServiceType.MobileCommunications,
                Accounts = new[] { "0700123456", "0700987654" }
            },
            new
            {
                Name = "MEGA",
                Type = ServiceType.MobileCommunications,
                Accounts = new[] { "0999123456", "0999987654" }
            },
            new
            {
                Name = "Aknet",
                Type = ServiceType.Internet,
                Accounts = new[] { "100001", "100002" }
            },
            new
            {
                Name = "Saima",
                Type = ServiceType.Internet,
                Accounts = new[] { "200001", "200002" }
            }
        };

        foreach (var providerData in providersData)
        {
            var provider = await context.ServiceProviders
                .FirstOrDefaultAsync(p => p.Name == providerData.Name && p.ServiceType == providerData.Type);

            if (provider == null)
            {
                provider = new ServiceProvider
                {
                    Name = providerData.Name,
                    ServiceType = providerData.Type
                };

                context.ServiceProviders.Add(provider);
                await context.SaveChangesAsync();
            }

            foreach (var accountNumber in providerData.Accounts)
            {
                var accountExists = await context.ProviderAccounts
                    .AnyAsync(a => a.ServiceProviderId == provider.Id && a.AccountNumber == accountNumber);

                if (!accountExists)
                {
                    var account = new ProviderAccount
                    {
                        AccountNumber = accountNumber,
                        Balance = 0,
                        ServiceProviderId = provider.Id
                    };

                    context.ProviderAccounts.Add(account);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}