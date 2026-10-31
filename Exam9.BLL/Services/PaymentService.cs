using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Interfaces;
using Exam9.DAL.Models;

namespace Exam9.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IProviderRepository _providerRepository;

    public PaymentService(IWalletRepository walletRepository, IProviderRepository providerRepository)
    {
        _walletRepository = walletRepository;
        _providerRepository = providerRepository;
    }

    public async Task<List<ServiceProvider>> GetProvidersAsync()
    {
        return await _providerRepository.GetAllAsync();
    }

    public async Task<PaymentResult> PayAsync(int userId, int providerId, string providerAccountNumber, decimal amount)
    {
        if (providerId <= 0)
            throw new ArgumentException("Выберите поставщика услуг.");

        if (string.IsNullOrWhiteSpace(providerAccountNumber))
            throw new ArgumentException("Введите номер лицевого счёта.");

        if (amount <= 0)
            throw new ArgumentException("Сумма должна быть больше нуля.");

        var wallet = await _walletRepository.GetByUserIdAsync(userId);

        if (wallet == null)
            return PaymentResult.WalletNotFound;

        return await _providerRepository.PayAsync(wallet.Id, providerId, providerAccountNumber.Trim(), amount);
    }
}