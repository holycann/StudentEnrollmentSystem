namespace StudentEnrollmentSystem.Models;

public class PaymentHistory
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionId { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public string PaymentMethod { get; set; }
    public string Notes { get; set; }
    public virtual ApplicationUser Student { get; set; }
}