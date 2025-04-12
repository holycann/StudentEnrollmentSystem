using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace StudentEnrollmentSystem.Models;

public class Semester
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int SemesterNumber { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();

    [JsonIgnore]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
}
