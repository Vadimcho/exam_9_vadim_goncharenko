namespace Exam9.DAL.Enums;

public enum TransferResult
{
    Success,
    SenderNotFound,
    ReceiverNotFound,
    InsufficientFunds,
    SameAccount
}