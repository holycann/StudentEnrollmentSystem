using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Models
{
    public class StudentEvaluation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required]
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public string Semester { get; set; }

        [Required]
        [Display(Name = "Year")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required]
        [Range(1, 5)]
        [Display(Name = "Course Content Rating")]
        public int CourseContentRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Teaching Quality Rating")]
        public int TeachingQualityRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Assessment Fairness Rating")]
        public int AssessmentFairnessRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Learning Resources Rating")]
        public int LearningResourcesRating { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Overall Satisfaction Rating")]
        public int OverallSatisfactionRating { get; set; }

        [Display(Name = "Strengths")]
        public string Strengths { get; set; }

        [Display(Name = "Areas for Improvement")]
        public string AreasForImprovement { get; set; }

        [Display(Name = "Additional Comments")]
        public string AdditionalComments { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        // Anonymous identifier to prevent duplicate submissions while maintaining anonymity
        public string AnonymousIdentifier { get; set; } = Guid.NewGuid().ToString();
    }
}