using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace StudentEnrollmentSystem.Models;

public class ProgramStudy
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public int DurationInYears { get; set; } // Durasi dalam tahun

    // Relasi ke Course
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();

    [JsonIgnore]
    public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
}