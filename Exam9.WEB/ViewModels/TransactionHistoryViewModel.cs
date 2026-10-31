using Exam9.DAL.Enums;

namespace Exam9.WEB.ViewModels;

public class TransactionHistoryViewModel
{
    public string AccountNumber { get; set; } = string.Empty;

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public List<TransactionHistoryItemViewModel> Transactions { get; set; } = new();
}

public class TransactionHistoryItemViewModel
{
    public DateTime CreatedAt { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsIncoming { get; set; }
}