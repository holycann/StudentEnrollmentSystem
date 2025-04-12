namespace StudentEnrollmentSystem.Models.Enums;

public enum PaymentMethod
{
    BankTransfer,
    Cards,
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled,
}
