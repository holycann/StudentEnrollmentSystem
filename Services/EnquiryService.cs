using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Services;

public interface IEnquiryService
{
    Task<(bool success, string message)> SubmitEnquiryAsync(
        string studentId,
        string subject,
        string message
    );
    Task<List<Enquiry>> GetEnquiryHistoryAsync(string studentId);
    Task<(bool success, string message)> SubmitEvaluationAsync(TeachingEvaluation evaluation);
    Task<List<TeachingEvaluation>> GetEvaluationHistoryAsync(string studentId);
    // Task<Dictionary<DayOfWeek, List<Models.ViewModels.TimeSlot>>> GetWeeklyScheduleAsync(
    //     string studentId
    // );
}

public class EnquiryService : IEnquiryService
{
    private readonly ApplicationDbContext _context;

    public EnquiryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool success, string message)> SubmitEnquiryAsync(
        string studentId,
        string subject,
        string message
    )
    {
        try
        {
            var enquiry = new Enquiry
            {
                StudentId = studentId,
                Subject = subject,
                Message = message,
                SubmissionDate = DateTime.UtcNow,
                Status = EnquiryStatus.Pending,
            };

            _context.Enquiries.Add(enquiry);
            await _context.SaveChangesAsync();
            return (true, "Enquiry submitted successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to submit enquiry: {ex.Message}");
        }
    }

    public async Task<List<Enquiry>> GetEnquiryHistoryAsync(string studentId)
    {
        return await _context
            .Enquiries.Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.SubmissionDate)
            .ToListAsync();
    }

    public async Task<(bool success, string message)> SubmitEvaluationAsync(
        TeachingEvaluation evaluation
    )
    {
        try
        {
            // Check if already submitted
            var existingEvaluation = await _context.TeachingEvaluations.AnyAsync(e =>
                e.CourseId == evaluation.CourseId && e.StudentId == evaluation.StudentId
            );

            if (existingEvaluation)
            {
                return (false, "You have already submitted an evaluation for this course");
            }

            evaluation.SubmittedAt = DateTime.UtcNow;
            _context.TeachingEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();
            return (true, "Evaluation submitted successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to submit evaluation: {ex.Message}");
        }
    }

    public async Task<List<TeachingEvaluation>> GetEvaluationHistoryAsync(string studentId)
    {
        return await _context
            .TeachingEvaluations.Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.SubmittedAt)
            .ToListAsync();
    }

    // public async Task<
    //     Dictionary<DayOfWeek, List<Models.ViewModels.TimeSlot>>
    // > GetWeeklyScheduleAsync(string studentId)
    // {
    //     var enrollments = await _context
    //         .Enrollments.Include(e => e.EnrollmentCourses)
    //         .ThenInclude(ec => ec.Course)
    //         .ThenInclude(c => c.CourseSchedules)
    //         .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active)
    //         .ToListAsync();

    //     var schedule = new Dictionary<DayOfWeek, List<Models.ViewModels.TimeSlot>>();
    //     foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
    //     {
    //         schedule[day] = new List<TimeSlot>();
    //     }

    //     foreach (var enrollment in enrollments)
    //     {
    //         foreach (
    //             var courseSchedule in enrollment.EnrollmentCourses.SelectMany(ec =>
    //                 ec.Course.CourseSchedules
    //             )
    //         )
    //         {
    //             schedule[courseSchedule.DayOfWeek]
    //                 .Add(
    //                     new TimeSlot
    //                     {
    //                         Day = courseSchedule.DayOfWeek,
    //                         StartTime = courseSchedule.StartTime,
    //                         EndTime = courseSchedule.EndTime,
    //                         Room = courseSchedule.Room,
    //                     }
    //                 );
    //         }
    //     }

    //     return schedule;
    // }
}
