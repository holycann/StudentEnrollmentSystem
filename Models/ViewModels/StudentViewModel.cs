using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class StudentViewModel
{
    [StringLength(20)]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public string ProgramName { get; set; } = string.Empty;

    [Required]
    public string ProgramCode { get; set; } = string.Empty;

    // Bank details (Opsional)
    [StringLength(25)] // Maksimum panjang nomor rekening
    public string? BankAccountNumber { get; set; }

    [StringLength(50)] // Maksimum panjang nama bank
    public string? BankName { get; set; }

    public virtual ICollection<EnrollmentViewModel> Enrollments { get; set; } =
        new HashSet<EnrollmentViewModel>();
}
