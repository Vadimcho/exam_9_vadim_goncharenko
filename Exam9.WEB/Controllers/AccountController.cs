using Exam9.BLL.Interfaces;
using Exam9.DAL.Models;
using Exam9.WEB.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Exam9.Resources;
using Microsoft.Extensions.Localization;

namespace Exam9.WEB.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;
    private readonly IStringLocalizer<AccountResource> _localizer;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IAccountService accountService, IStringLocalizer<AccountResource> localizer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _localizer = localizer;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string accountNumber = await _accountService.GenerateAccountNumberAsync();

        var user = new User
        {
            UserName = accountNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        try
        {
            await _accountService.CreateWalletAsync(user.Id, accountNumber);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception)
        {
            await _userManager.DeleteAsync(user);

            ModelState.AddModelError(string.Empty, _localizer["WalletCreationFailed"].Value);

            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string accountNumber, string password)
    {
        var user = await _userManager.FindByNameAsync(accountNumber);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, _localizer["InvalidCredentials"].Value);
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError(string.Empty, _localizer["InvalidCredentials"].Value);

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
}