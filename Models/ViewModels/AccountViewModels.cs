using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class UpdateProfileViewModels
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
}

public class UpdateBankDetailsViewModels
{
    [Required(ErrorMessage = "Bank Name is required")]
    public string BankName { get; set; }

    [Required(ErrorMessage = "Account Number is required")]
    public string AccountNumber { get; set; }

}

public class ChangePasswordViewModels
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    public string Message { get; set; }
}