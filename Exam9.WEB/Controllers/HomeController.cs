using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Models;
using Exam9.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Exam9.WEB.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletService _walletService;

    public HomeController(UserManager<User> userManager, IWalletService walletService)
    {
        _userManager = userManager;
        _walletService = walletService;
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

            if (!result)
            {
                return Json(new
                {
                    success = false,
                    message = "Счёт не найден."
                });
            }

            return Json(new
            {
                success = true,
                message = "Счёт успешно пополнен."
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
                    message = "Пользователь не найден."
                });
            }

            var result = await _walletService.TransferAsync(user.Id, receiverAccountNumber, amount);

            var message = result switch
            {
                TransferResult.Success => "Перевод успешно выполнен.",
                TransferResult.ReceiverNotFound => "Счёт получателя не найден.",
                TransferResult.SenderNotFound => "Ваш кошелёк не найден.",
                TransferResult.InsufficientFunds => "Недостаточно средств.",
                TransferResult.SameAccount => "Нельзя переводить деньги самому себе.",
                _ => "Не удалось выполнить перевод."
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

        return Json(new
        {
            balance = wallet.Balance
        });
    }
}