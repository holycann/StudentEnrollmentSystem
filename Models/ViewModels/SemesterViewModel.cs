using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class SemesterViewModel
{
    public string Name { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
