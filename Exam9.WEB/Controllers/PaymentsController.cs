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
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly UserManager<User> _userManager;
    private readonly IStringLocalizer<PaymentResource> _localizer;

    public PaymentsController(IPaymentService paymentService, UserManager<User> userManager, IStringLocalizer<PaymentResource> localizer)
    {
        _paymentService = paymentService;
        _userManager = userManager;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var providers = await _paymentService.GetProvidersAsync();

        var model = new PaymentViewModel
        {
            Providers = providers.Select(p => new ProviderOptionViewModel
            {
                Id = p.Id,
                Name = p.Name,
                ServiceType = p.ServiceType switch
                {
                    ServiceType.MobileCommunications => _localizer["MobileCommunications"].Value,
                    ServiceType.Internet => _localizer["Internet"].Value,
                    ServiceType.Television => _localizer["Television"].Value,
                    _ => _localizer["Other"].Value
                }
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(PaymentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault() ?? _localizer["CheckInput"].Value
                });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _paymentService.PayAsync(user.Id, model.ProviderId, model.ProviderAccountNumber, model.Amount);

            var message = result switch
            {
                PaymentResult.Success => _localizer["PaymentSuccess"].Value,
                PaymentResult.WalletNotFound => _localizer["WalletNotFound"].Value,
                PaymentResult.ProviderNotFound => _localizer["ProviderNotFound"].Value,
                PaymentResult.AccountNotFound => _localizer["AccountNotFound"].Value,
                PaymentResult.InsufficientFunds => _localizer["InsufficientFunds"].Value,
                _ => _localizer["PaymentFailed"].Value
            };

            return Json(new
            {
                success = result == PaymentResult.Success,
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
}