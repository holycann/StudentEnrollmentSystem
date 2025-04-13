
using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class AvailableCourseViewModel
{
    public IList<Course> Courses { get; set; } = default!;
    public Student CurrentStudent { get; set; } = default!;

    public string CurrentFilter { get; set; } = string.Empty;
    public string CurrentSort { get; set; } = string.Empty;
    public string CurrentDepartmentFilter { get; set; } = "all";

    public string NameSort { get; set; } = string.Empty;
    public string CodeSort { get; set; } = string.Empty;
    public string CreditsSort { get; set; } = string.Empty;

    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

