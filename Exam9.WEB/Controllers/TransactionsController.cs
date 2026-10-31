using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Models;
using Exam9.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Exam9.WEB.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletService _walletService;
    private readonly ITransactionService _transactionService;

    public TransactionsController(UserManager<User> userManager, IWalletService walletService, ITransactionService transactionService)
    {
        _userManager = userManager;
        _walletService = walletService;
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var wallet = await _walletService.GetByUserIdAsync(user.Id);

        if (wallet == null)
            return NotFound();

        if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
        {
            ModelState.AddModelError("", "Начальная дата не может быть позже конечной.");
            dateFrom = null;
            dateTo = null;
        }

        var transactions = await _transactionService.GetByWalletIdAsync(wallet.Id, dateFrom, dateTo);

        var model = new TransactionHistoryViewModel
        {
            AccountNumber = wallet.AccountNumber,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Transactions = transactions.Select(t =>
            {
                var isIncoming = t.ReceiverWalletId == wallet.Id;

                var description = t.Type switch
                {
                    TransactionType.Deposit => "Пополнение счёта",
                    TransactionType.Transfer when t.SenderWalletId == wallet.Id => $"Перевод на счёт {t.ReceiverWallet?.AccountNumber}",
                    TransactionType.Transfer => $"Перевод от счёта {t.SenderWallet?.AccountNumber}",
                    TransactionType.Payment => $"Оплата: {t.ProviderAccount?.ServiceProvider?.Name}",
                    _ => "Операция"
                };

                return new TransactionHistoryItemViewModel
                {
                    CreatedAt = t.CreatedAt,
                    Amount = t.Amount,
                    Type = t.Type,
                    Description = description,
                    IsIncoming = isIncoming
                };
            }).ToList()
        };

        return View(model);
    }
}