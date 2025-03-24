using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services;

public interface IPaymentService
{
    Task<(bool success, string? transactionId)> ProcessPaymentAsync(decimal amount, string cardNumber, string expiryDate, string cvv, string cardholderName);
    Task<bool> RefundPaymentAsync(string transactionId);
}

public class DummyPaymentService : IPaymentService
{
    private readonly Random _random = new Random();

    public async Task<(bool success, string? transactionId)> ProcessPaymentAsync(decimal amount, string cardNumber, string expiryDate, string cvv, string cardholderName)
    {
        // Simulate API call delay
        await Task.Delay(1000);

        // Dummy validation
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length != 16)
            return (false, null!);

        if (string.IsNullOrEmpty(expiryDate) || !expiryDate.Contains('/'))
            return (false, null!);

        if (string.IsNullOrEmpty(cvv) || cvv.Length != 3)
            return (false, null!);

        if (string.IsNullOrEmpty(cardholderName))
            return (false, null!);

        // 90% success rate for dummy payments
        bool isSuccessful = _random.Next(100) < 90;
        string? transactionId = isSuccessful ? GenerateTransactionId() : null;

        return (isSuccessful, transactionId);
    }

    public async Task<bool> RefundPaymentAsync(string transactionId)
    {
        // Simulate API call delay
        await Task.Delay(1000);

        // 95% success rate for dummy refunds
        return _random.Next(100) < 95;
    }

    private string GenerateTransactionId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
