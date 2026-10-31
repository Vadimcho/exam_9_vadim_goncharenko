using Exam9.BLL.Interfaces;
using Exam9.DAL.Models;
using Exam9.WEB.ViewModels;
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
}