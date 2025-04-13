using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class EnrollmentViewModel
{
    public IList<Enrollment> Enrollments { get; set; } = default!;
    public Student CurrentStudent { get; set; } = default!;
}