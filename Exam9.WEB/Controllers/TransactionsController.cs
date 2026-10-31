using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Models;
using Exam9.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Exam9.Resources;
using Microsoft.Extensions.Localization;

namespace Exam9.WEB.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletService _walletService;
    private readonly ITransactionService _transactionService;
    private readonly IStringLocalizer<TransactionResource> _localizer;

    public TransactionsController(UserManager<User> userManager, IWalletService walletService, ITransactionService transactionService, IStringLocalizer<TransactionResource> localizer)
    {
        _userManager = userManager;
        _walletService = walletService;
        _transactionService = transactionService;
        _localizer = localizer;
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
            ModelState.AddModelError("", _localizer["InvalidDateRange"].Value);
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
                    TransactionType.Deposit => _localizer["DepositDescription"].Value,
                    TransactionType.Transfer when t.SenderWalletId == wallet.Id => _localizer["TransferTo", t.ReceiverWallet?.AccountNumber].Value,
                    TransactionType.Transfer => _localizer["TransferFrom", t.SenderWallet?.AccountNumber].Value,
                    TransactionType.Payment => _localizer["PaymentDescription", t.ProviderAccount?.ServiceProvider?.Name].Value,
                    _ => _localizer["OtherOperation"].Value
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