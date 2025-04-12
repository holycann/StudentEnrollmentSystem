using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class TimetableMatchingViewModel
{
    public TimetableMatchRequest TimetableRequest { get; set; } = new TimetableMatchRequest();
    public List<string> SelectedCourses { get; set; } = new List<string>();
    public List<string> UnavailableTimes { get; set; } = new List<string>();
    public List<string> AvailableCourses { get; set; } = new List<string>();
}
