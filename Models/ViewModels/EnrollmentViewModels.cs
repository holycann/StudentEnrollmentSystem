using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels
{
    public class CourseEnrollmentViewModel
    {
        public int CourseId { get; set; }
        public required string CourseCode { get; set; }
        public required string CourseName { get; set; }
        public required string Description { get; set; }
        public int CreditHours { get; set; }
        public int Credits => CreditHours;
        public decimal Fee { get; set; }
        public required string Schedule { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public bool IsPrerequisiteMet { get; set; }
        public bool IsAlreadyEnrolled { get; set; }
        public bool IsEnrolled => IsAlreadyEnrolled;
        public bool CanEnroll => !IsAlreadyEnrolled && IsPrerequisiteMet && AvailableSeats > 0;
        public List<string> MissingPrerequisites { get; set; } = new();
        public List<string> PrerequisitesCodes { get; set; } = new();
        public int AvailableSeats => Capacity - EnrolledCount;
    }

    public class DropCourseViewModel
    {
        public int EnrollmentId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Schedule { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal RefundAmount { get; set; }
        
        [Required]
        [Display(Name = "Reason for Dropping")]
        public string DropReason { get; set; }
    }
}
