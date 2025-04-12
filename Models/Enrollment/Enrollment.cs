using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;
public class Enrollment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Student")]
    public string StudentId { get; set; } = string.Empty;

    [JsonIgnore]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("Program")]
    public int ProgramId { get; set; }
    public virtual ProgramStudy Program { get; set; } = null!;

    [ForeignKey("Semester")]
    public int SemesterId { get; set; }
    public virtual Semester Semester { get; set; } = null!;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    public int TotalCredits { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public virtual ICollection<EnrollmentCourse> EnrollmentCourses { get; set; } =
        new HashSet<EnrollmentCourse>();

    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}
