using Exam9.BLL.Interfaces;
using Exam9.DAL.Enums;
using Exam9.DAL.Models;
using Exam9.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Exam9.WEB.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly UserManager<User> _userManager;

    public PaymentsController(IPaymentService paymentService, UserManager<User> userManager)
    {
        _paymentService = paymentService;
        _userManager = userManager;
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
                    ServiceType.MobileCommunications => "Мобильная связь",
                    ServiceType.Internet => "Интернет",
                    ServiceType.Television => "Телевидение",
                    _ => "Другое"
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
                        .FirstOrDefault() ?? "Проверьте введённые данные."
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
                PaymentResult.Success => "Оплата успешно выполнена.",
                PaymentResult.WalletNotFound => "Ваш кошелёк не найден.",
                PaymentResult.ProviderNotFound => "Поставщик услуг не найден.",
                PaymentResult.AccountNotFound => "Лицевой счёт у поставщика не найден.",
                PaymentResult.InsufficientFunds => "Недостаточно средств на балансе.",
                _ => "Не удалось выполнить оплату."
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