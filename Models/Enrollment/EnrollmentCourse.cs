using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models
{
    public class EnrollmentCourse
    {
        [Key]
        public int Id { get; set; }

        // Foreign key ke Enrollment
        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; } = null!;

        // Foreign key ke Course
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        public EnrollmentCourseStatus Status { get; set; } = EnrollmentCourseStatus.Enrolled;

        [Range(0, 100)]
        public decimal? Grade { get; set; }
    }
}
