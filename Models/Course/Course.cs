using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace StudentEnrollmentSystem.Models;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public decimal FeePerCredit { get; set; }

    // Foreign Key ke Teacher
    [ForeignKey("Teacher")]
    public string TeacherId { get; set; }
    public virtual Teacher Teacher { get; set; } = null!;

    [Required]
    public int ProgramId { get; set; }
    [ForeignKey("ProgramId")]
    [JsonIgnore]
    public virtual ProgramStudy ProgramStudy { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<EnrollmentCourse> EnrollmentCourses { get; set; } =
        new HashSet<EnrollmentCourse>();
    public virtual ICollection<CourseSchedule> CourseSchedules { get; set; } =
        new HashSet<CourseSchedule>();
    public virtual ICollection<CoursePrerequisite> PrerequisiteCourses { get; set; } =
        new HashSet<CoursePrerequisite>();
    public virtual ICollection<CoursePrerequisite> CoursesRequiringThis { get; set; } =
        new HashSet<CoursePrerequisite>();
}
