namespace Exam9.BLL.Interfaces;

public interface IAccountService
{
    Task<string> GenerateAccountNumberAsync();
    Task CreateWalletAsync(int userId, string accountNumber);
}