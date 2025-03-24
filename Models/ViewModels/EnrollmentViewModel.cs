using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class EnrollmentViewModel
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    public string StudentId { get; set; }

    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; }

    public Enums.EnrollmentStatus Status { get; set; }

    public string Notes { get; set; }

    // Navigation properties
    public CourseViewModel Course { get; set; }
    public string StudentName { get; set; }
}

public class EnrollmentRequestViewModel
{
    public int CourseId { get; set; }
    public CourseViewModel Course { get; set; }
    public bool HasPrerequisites { get; set; }
    public List<string> MissingPrerequisites { get; set; }
    public decimal TotalFee { get; set; }
}

public class EnrollmentHistoryViewModel
{
    public IEnumerable<EnrollmentViewModel> CurrentEnrollments { get; set; }
    public IEnumerable<EnrollmentViewModel> PastEnrollments { get; set; }
    public decimal TotalFees { get; set; }
    public int TotalCredits { get; set; }
}
