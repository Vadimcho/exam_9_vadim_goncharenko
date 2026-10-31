
using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Models;
using Exam9.Resources;
using Exam9.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Exam9.WEB.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletService _walletService;
    private readonly IStringLocalizer<WalletResource> _localizer;

    public HomeController(UserManager<User> userManager, IWalletService walletService, IStringLocalizer<WalletResource> localizer)
    {
        _userManager = userManager;
        _walletService = walletService;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel();

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var wallet = await _walletService.GetByUserIdAsync(user.Id);

                if (wallet != null)
                {
                    model.AccountNumber = wallet.AccountNumber;
                    model.Balance = wallet.Balance;
                }
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deposit(string accountNumber, decimal amount)
    {
        try
        {
            var result = await _walletService.DepositAsync(accountNumber, amount);

            return Json(new
            {
                success = result,
                message = result ? _localizer["DepositSuccess"].Value : _localizer["AccountNotFound"].Value
            });
        }
        catch (ArgumentException ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(string receiverAccountNumber, decimal amount)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = _localizer["UserNotFound"].Value
                });
            }

            var result = await _walletService.TransferAsync(user.Id, receiverAccountNumber, amount);

            var message = result switch
            {
                TransferResult.Success => _localizer["TransferSuccess"].Value,
                TransferResult.ReceiverNotFound => _localizer["ReceiverNotFound"].Value,
                TransferResult.SenderNotFound => _localizer["SenderNotFound"].Value,
                TransferResult.InsufficientFunds => _localizer["InsufficientFunds"].Value,
                TransferResult.SameAccount => _localizer["SameAccount"].Value,
                _ => _localizer["TransferFailed"].Value
            };

            return Json(new
            {
                success = result == TransferResult.Success,
                message
            });
        }
        catch (ArgumentException ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetBalance()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var wallet = await _walletService.GetByUserIdAsync(user.Id);

        if (wallet == null)
            return NotFound();

        return Json(new { balance = wallet.Balance });
    }
}
