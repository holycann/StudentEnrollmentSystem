using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class EnquiryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    [StringLength(200, ErrorMessage = "Subject cannot be longer than 200 characters")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Message is required")]
    [MinLength(10, ErrorMessage = "Message must be at least 10 characters long")]
    public string Message { get; set; }

    public Enums.EnquiryStatus Status { get; set; }
    public string Response { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TeachingEvaluationViewModel
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public string InstructorName { get; set; } = string.Empty;

    [Required]
    [Range(1, 5)]
    public int TeachingQuality { get; set; }

    [Required]
    [Range(1, 5)]
    public int MaterialQuality { get; set; }

    [Required]
    [Range(1, 5)]
    public int AssignmentFeedback { get; set; }

    [Required]
    [Range(1, 5)]
    public int Engagement { get; set; }

    [StringLength(1000)]
    public string Comments { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }
}

public class TimetableViewModel
{
    public List<CourseScheduleViewModel> Courses { get; set; }
    public Dictionary<string, List<TimeSlot>> Schedule { get; set; }
}

public class CourseScheduleViewModel
{
    public int CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public string Room { get; set; }
    public string DayAndTime { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public List<TimeSlot> TimeSlots { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class TimeSlot
{
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Room { get; set; }
    public string Type { get; set; } // Lecture, Tutorial, Lab
}
