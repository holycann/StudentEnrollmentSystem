using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudentEnrollmentSystem.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        
        [Required]
        [ForeignKey("StudentId")]
        public string StudentId { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        
        [Required]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        public string Status { get; set; } = "Enrolled"; // Enrolled, Dropped, Completed
        
        // Navigation properties
        [JsonIgnore]
        public virtual Student? Student { get; set; }
        
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
}