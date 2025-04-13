using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentEnrollmentSystem.Models;

namespace Entityonlineform.Models
{
    public class AddDropRecord
    {
        [Key]
        public int AddDropId { get; set; }
        
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        
        [Required]
        public string Action { get; set; } = string.Empty; // "Add" or "Drop"
        
        [Required]
        public DateTime ActionDate { get; set; } = DateTime.Now;
        
        [StringLength(500)]
        public string? Reason { get; set; }
        
        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
}