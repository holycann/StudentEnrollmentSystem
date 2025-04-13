using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class MakePaymentViewModel
{
    public int PaymentId { get; set; }
    public string StudentId { get; set; }
    public int SemesterId { get; set; }
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }

    [Display(Name = "Card Number")]
    public string? CardNumber { get; set; }

    [Display(Name = "Expiry Date")]
    public string? ExpiryDate { get; set; }

    [Display(Name = "CVV")]
    public string? CVV { get; set; }

    [Display(Name = "Cardholder Name")]
    public string? CardholderName { get; set; }

    public StudentViewModel? StudentViewModel { get; set; }
    public List<Course> Courses { get; set; } = new List<Course>();
}

public class WaitingPaymentViewModel
{
    public int PaymentId { get; set; }
    public string StudentName { get; set; }
    public DateTime ExpirationDate { get; set; }
    public MakePaymentViewModel MakePaymentViewModel { get; set; }
}

public class PaymentDetailsViewModel
{
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }

    // Student Data
    public string StudentName { get; set; }
    public string StudentId { get; set; }
    public string Email { get; set; }

    // Payment Data
    public int PaymentId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string PaymentMethod { get; set; }

    // Courses Data
    public List<Course> Courses { get; set; }
}

public class PaymentHistoryViewModel
{
    public int PaymentId { get; set; }
    public string SemesterName { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentInvoiceViewModel
{
    public int PaymentId { get; set; }
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
    public string ProgramName { get; set; }
    public string SemesterName { get; set; }

    // Payment Data
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Courses Data
    public List<Course> Courses { get; set; }
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
    public List<SelectListItem>? Courses { get; set; }
    public List<SelectListItem>? Programs { get; set; }
    public List<SelectListItem>? Semesters { get; set; }
    public List<SelectListItem>? PaymentMethods { get; set; }
}