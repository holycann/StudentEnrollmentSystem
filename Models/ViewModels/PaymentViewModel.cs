using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class MakePaymentViewModel
{
    public int PaymentId { get; set; }
    public string StudentId { get; set; }
    public int EnrollmentId { get; set; }
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }

    [Display(Name = "Card Number")]
    // [RegularExpression(@"^\d{16}$", ErrorMessage = "Please enter a valid 16-digit card number")]
    public string? CardNumber { get; set; }

    [Display(Name = "Expiry Date")]
    // [RegularExpression(
    //     @"^(0[1-9]|1[0-2])\/([0-9]{2})$",
    //     ErrorMessage = "Please enter a valid expiry date (MM/YY)"
    // )]
    public string? ExpiryDate { get; set; }

    [Display(Name = "CVV")]
    // [RegularExpression(@"^\d{3}$", ErrorMessage = "Please enter a valid 3-digit CVV")]
    public string? CVV { get; set; }

    [Display(Name = "Cardholder Name")]
    public string? CardholderName { get; set; }

    // Navigation properties
    public EnrollmentViewModel? Enrollment { get; set; }
    public StudentViewModel? Student { get; set; }
}

public class WaitingPaymentViewModel
{
    public int PaymentId { get; set; }
    public string StudentName { get; set; }
    public DateTime ExpirationDate { get; set; }
    public MakePaymentViewModel Payment { get; set; }
}

public class PaymentDetailsViewModel
{
    // Student Data
    public string StudentName { get; set; }
    public string StudentId { get; set; }
    public string Email { get; set; }

    // Enrollment Data
    public int EnrollmentId { get; set; }
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; }

    // Payment Data
    public int PaymentId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }

    // EnrollmentCourses
    public virtual ICollection<EnrollmentCourseViewModel> EnrollmentCourses { get; set; }
}

public class PaymentHistoryViewModel
{
    public int PaymentId { get; set; }
    public int EnrollmentId { get; set; }
    public string SemesterName { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentInvoiceViewModel
{
    public int PaymentId { get; set; }
    public int EnrollmentId { get; set; }
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentInvoiceDetailsViewModel
{
    // Student Data
    public string StudentName { get; set; }
    public string StudentId { get; set; }
    public string Email { get; set; }

    // Enrollment Data
    public int EnrollmentId { get; set; }
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }
    public DateTime EnrollmentDate { get; set; }

    // Payment Data
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime ExpirationDate { get; set; }

    // EnrollmentCourses
    public virtual ICollection<EnrollmentCourseViewModel> EnrollmentCourses { get; set; }
}

public class PaymentAdjustmentViewModel
{
    public int PaymentId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int ProgramId { get; set; }
    public int SemesterId { get; set; }
    public List<int> SelectedCourses { get; set; }

    public string? StudentName { get; set; }
    public string? StudentId { get; set; }
    public string? Email { get; set; }
    public int? TotalCredits { get; set; }
    public string? Status { get; set; }
    public string? ProgramName { get; set; }
    public string? SemesterName { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public virtual ICollection<EnrollmentCourseViewModel>? EnrollmentCourses { get; set; }
    public List<SelectListItem>? Courses { get; set; }
    public List<SelectListItem>? Programs { get; set; }
    public List<SelectListItem>? Semesters { get; set; }
    public List<SelectListItem>? PaymentMethods { get; set; }
}