using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class StatementViewModel
{
    public required string StudentId { get; set; }
    public required string StudentName { get; set; }
    public required string StudentEmail { get; set; }
    public required string Program { get; set; }
    public DateTime GeneratedDate { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance => TotalFees - TotalPaid;
    public string Description { get; set; } = string.Empty;
    public List<EnrollmentStatement> Enrollments { get; set; } = new();
    public List<PaymentStatement> Payments { get; set; } = new();
    public required RegistrationSummary Registration { get; set; }
}

public class EnrollmentStatement
{
    public int EnrollmentId { get; set; }
    public required string CourseCode { get; set; }
    public required string CourseName { get; set; }
    public int Credits { get; set; }
    public decimal Fee { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public Enums.EnrollmentStatus Status { get; set; }
    public List<TimeSlot> Schedule { get; set; }
}

public class PaymentStatement
{
    public int PaymentId { get; set; }
    public required string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public required string PaymentMethod { get; set; }
}

public class RegistrationSummary
{
    public int TotalCredits { get; set; }
    public int TotalCourses { get; set; }
    public DateTime RegistrationDate { get; set; }
    public required string AcademicTerm { get; set; }
    public List<string> EnrolledCourses { get; set; } = new();
    public Dictionary<DayOfWeek, List<TimeSlot>> WeeklySchedule { get; set; } = new();
}
