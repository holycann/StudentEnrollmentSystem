using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models
{
    public class Timetable
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required]
        [Display(Name = "Day of Week")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Room")]
        public string Room { get; set; }

        [Required]
        [Display(Name = "Instructor")]
        public string Instructor { get; set; }

        [Display(Name = "Semester")]
        public string Semester { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; } = DateTime.Now.Year;
    }

    public class TimetableMatchRequest
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; }

        [Required]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Preferred Courses")]
        public List<string> PreferredCourses { get; set; } = new List<string>();

        [Display(Name = "Unavailable Days/Times")]
        public List<UnavailableTime> UnavailableTimes { get; set; } = new List<UnavailableTime>();

        [Display(Name = "Additional Notes")]
        public string AdditionalNotes { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;
    }

    public class UnavailableTime
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}