using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class PaymentViewModel
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public int EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string TransactionId { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public string PaymentMethod { get; set; }

    // Navigation properties
    public EnrollmentViewModel Enrollment { get; set; }
    public string StudentName { get; set; }
}

public class PaymentRequestViewModel
{
    public int EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }

    [Required]
    [Display(Name = "Card Number")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Please enter a valid 16-digit card number")]
    public string CardNumber { get; set; }

    [Required]
    [Display(Name = "Expiry Date")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Please enter a valid expiry date (MM/YY)")]
    public string ExpiryDate { get; set; }

    [Required]
    [Display(Name = "CVV")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Please enter a valid 3-digit CVV")]
    public string CVV { get; set; }

    [Required]
    [Display(Name = "Cardholder Name")]
    public string CardholderName { get; set; }
}

public class PaymentHistoryViewModel
{
    public IEnumerable<PaymentViewModel> Payments { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal PendingPayments { get; set; }
}