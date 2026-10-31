using System.ComponentModel.DataAnnotations;

namespace Exam9.WEB.ViewModels;

public class PaymentViewModel
{
    [Required(ErrorMessage = "Выберите поставщика услуг.")]
    public int ProviderId { get; set; }

    [Required(ErrorMessage = "Введите номер лицевого счёта.")]
    public string ProviderAccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите сумму.")]
    [Range(0.01, 100000000, ErrorMessage = "Сумма должна быть больше нуля.")]
    public decimal Amount { get; set; }

    public List<ProviderOptionViewModel> Providers { get; set; } = new();
}

public class ProviderOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
}