using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models;

public class CoursePrerequisite
{
    [Key]
    public int Id { get; set; }

    // Foreign Key ke Course utama
    [ForeignKey("Course")]
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("PrerequisiteCourse")]
    public int PrerequisiteCourseId { get; set; }
    public virtual Course PrerequisiteCourses { get; set; } = null!;

    // Foreign Key ke Semester
    [ForeignKey("Semester")]
    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; } = null!;
}
