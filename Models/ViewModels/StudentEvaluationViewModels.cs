using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class StudentEvaluationViewModels
{
    public StudentEvaluation StudentEvaluation { get; set; }
    public Dictionary<string, string>? AvailableCourses { get; set; }
    public Dictionary<string, CourseInfo>? CourseData { get; set; }
}

public class CourseInfo
{
    public string Name { get; set; }
    public string Instructor { get; set; }
}