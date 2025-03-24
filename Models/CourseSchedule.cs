using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models;

public class CourseSchedule
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }

    [Required]
    [StringLength(50)]
    public string Room { get; set; } = string.Empty;

    [Required]
    public string InstructorName { get; set; } = string.Empty;

    public string? Notes { get; set; }
}
