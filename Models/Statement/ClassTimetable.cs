using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace StudentEnrollmentSystem.Models;

public class ClassSchedule
{
    public string Day { get; set; }
    public string Time { get; set; }
    public string Course { get; set; }
    public string Lecturer { get; set; }
    public string Room { get; set; }
}

public class ClassTimetable
{
    public List<ClassSchedule> Classes { get; set; }
}