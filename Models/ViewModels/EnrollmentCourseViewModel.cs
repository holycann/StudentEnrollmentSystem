using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class EnrollmentCourseViewModel
{
    public int EnrollmentId { get; set; }
    public virtual EnrollmentViewModel Enrollment { get; set; } = null!;

    public int CourseId { get; set; }
    public virtual CourseViewModel Course { get; set; } = null!;

    public Enums.EnrollmentCourseStatus Status { get; set; } =
        Enums.EnrollmentCourseStatus.Enrolled;

    [Range(0, 100)]
    public decimal? Grade { get; set; }
}
