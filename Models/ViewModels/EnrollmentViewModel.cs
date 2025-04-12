using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class EnrollmentViewModel
{
    public int Id { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    public int TotalCredits { get; set; }
    public decimal TotalFees { get; set; }
    public DateTime? EnrollmentDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<EnrollmentCourseViewModel>? EnrollmentCourses { get; set; } =
        new HashSet<EnrollmentCourseViewModel>();
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
