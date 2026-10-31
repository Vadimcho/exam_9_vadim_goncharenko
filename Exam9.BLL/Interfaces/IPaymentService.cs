using Exam9.DAL.Enums;
using Exam9.DAL.Models;

namespace Exam9.BLL.Interfaces;

public interface IPaymentService
{
    Task<List<ServiceProvider>> GetProvidersAsync();
    Task<PaymentResult> PayAsync(int userId, int providerId, string providerAccountNumber, decimal amount);
}