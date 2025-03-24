namespace StudentEnrollmentSystem.Models;

public class Course
{
    public int Id { get; set; } = 0;
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; } = 0;
    public int MaxSeats { get; set; } = 0;
    public int AvailableSeats { get; set; } = 0;
    public decimal Fee { get; set; } = 0;
    public string Prerequisites { get; set; } = string.Empty;
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    public virtual ICollection<Course> PrerequisiteCourses { get; set; } = new HashSet<Course>();
    public virtual ICollection<CourseSchedule> CourseSchedules { get; set; } = new HashSet<CourseSchedule>();
}
