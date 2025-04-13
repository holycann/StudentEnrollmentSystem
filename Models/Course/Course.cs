using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace StudentEnrollmentSystem.Models;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required]
    [StringLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string CourseCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int Credits { get; set; }
    public decimal Fee { get; set; }

    [ForeignKey("Semester")]
    public int SemesterId  { get; set; }

    [ForeignKey("ProgramStudy")]
    public int ProgramStudyId  { get; set; }

    public int Capacity { get; set; }

    public int EnrolledCount { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
