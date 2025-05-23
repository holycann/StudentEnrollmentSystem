using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class ProgramStudyViewModel
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationInYears { get; set; }
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
