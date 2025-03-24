using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class ProfileViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }

    [Required]
    public string Program { get; set; }

    [Display(Name = "Student ID")]
    public string StudentId { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile ProfilePicture { get; set; }
}

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}

public class BankDetailsViewModel
{
    [Required]
    [Display(Name = "Bank Name")]
    public string BankName { get; set; }

    [Required]
    [Display(Name = "Account Number")]
    [RegularExpression(@"^\d{10,20}$", ErrorMessage = "Please enter a valid bank account number.")]
    public string AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account Holder Name")]
    public string AccountHolderName { get; set; }

    [Display(Name = "Branch Name")]
    public string BranchName { get; set; }

    [Display(Name = "Swift Code")]
    [RegularExpression(@"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$", ErrorMessage = "Please enter a valid SWIFT code.")]
    public string SwiftCode { get; set; }
}

public class SecurityPreferencesViewModel
{
    [Display(Name = "Enable Two-Factor Authentication")]
    public bool EnableTwoFactor { get; set; }

    [Display(Name = "Receive Security Notifications")]
    public bool ReceiveSecurityNotifications { get; set; }

    [Display(Name = "Login Alert Emails")]
    public bool LoginAlertEmails { get; set; }

    [Display(Name = "Remember Browser")]
    public bool RememberBrowser { get; set; }
}

public class NotificationPreferencesViewModel
{
    [Display(Name = "Email Notifications")]
    public bool EmailNotifications { get; set; }

    [Display(Name = "SMS Notifications")]
    public bool SmsNotifications { get; set; }

    [Display(Name = "Payment Reminders")]
    public bool PaymentReminders { get; set; }

    [Display(Name = "Course Updates")]
    public bool CourseUpdates { get; set; }

    [Display(Name = "Enrollment Notifications")]
    public bool EnrollmentNotifications { get; set; }
}
