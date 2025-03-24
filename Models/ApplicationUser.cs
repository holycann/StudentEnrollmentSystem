using Microsoft.AspNetCore.Identity;

namespace StudentEnrollmentSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string? StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Program { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool ReceiveSecurityNotifications { get; set; }
    public bool LoginAlertEmails { get; set; }

    public bool EmailNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool PaymentReminders { get; set; }
    public bool CourseUpdates { get; set; }
    public bool EnrollmentNotifications { get; set; }
    public bool RememberBrowser { get; set; }
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string ProfilePicturePath { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Enrollment> Enrollments { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }

    public virtual ICollection<Enquiry> Enquiries { get; set; }
}
