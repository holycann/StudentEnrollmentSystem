using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Services;

public interface IEnrollmentService
{
    Task<(bool canEnroll, List<string> missingPrerequisites)> CanEnrollInCourseAsync(string studentId, int courseId);
    Task<bool> HasScheduleConflictAsync(string studentId, int courseId);
    Task<(bool success, string message)> EnrollInCourseAsync(string studentId, int courseId);
    Task<(bool success, string message)> DropCourseAsync(int enrollmentId, string reason);
    Task<List<Course>> GetAvailableCoursesAsync(string studentId);
    Task<List<Enrollment>> GetCurrentEnrollmentsAsync(string studentId);
    Task<List<Enrollment>> GetEnrollmentHistoryAsync(string studentId);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool canEnroll, List<string> missingPrerequisites)> CanEnrollInCourseAsync(string studentId, int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.PrerequisiteCourses)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return (false, new List<string> { "Course not found" });
        }

        if (course.AvailableSeats <= 0)
        {
            return (false, new List<string> { "No available seats" });
        }

        // Check if already enrolled
        var existingEnrollment = await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && 
                         (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending));
        
        if (existingEnrollment)
        {
            return (false, new List<string> { "Already enrolled in this course" });
        }

        // Check prerequisites
        var missingPrerequisites = new List<string>();
        foreach (var prerequisite in course.PrerequisiteCourses)
        {
            var completed = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && 
                              e.CourseId == prerequisite.Id && 
                              e.Status == EnrollmentStatus.Completed);
            
            if (!completed)
            {
                missingPrerequisites.Add(prerequisite.CourseCode);
            }
        }

        return (missingPrerequisites.Count == 0, missingPrerequisites);
    }

    public async Task<bool> HasScheduleConflictAsync(string studentId, int courseId)
    {
        var targetCourse = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (targetCourse == null)
        {
            return false;
        }

        var currentEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId && 
                       (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending))
            .ToListAsync();

        // TODO: Implement actual schedule conflict checking logic
        // For now, we'll assume no conflicts
        return false;
    }

    public async Task<(bool success, string message)> EnrollInCourseAsync(string studentId, int courseId)
    {
        var (canEnroll, missingPrerequisites) = await CanEnrollInCourseAsync(studentId, courseId);
        
        if (!canEnroll)
        {
            return (false, $"Cannot enroll: {string.Join(", ", missingPrerequisites)}");
        }

        if (await HasScheduleConflictAsync(studentId, courseId))
        {
            return (false, "Schedule conflict detected");
        }

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
        {
            return (false, "Course not found");
        }

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Pending,
            Notes = ""
        };

        _context.Enrollments.Add(enrollment);
        course.AvailableSeats--;
        
        await _context.SaveChangesAsync();
        return (true, "Enrollment successful");
    }

    public async Task<(bool success, string message)> DropCourseAsync(int enrollmentId, string reason)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null)
        {
            return (false, "Enrollment not found");
        }

        if (enrollment.Status == EnrollmentStatus.Dropped || enrollment.Status == EnrollmentStatus.Completed)
        {
            return (false, "Cannot drop this enrollment");
        }

        enrollment.Status = EnrollmentStatus.Dropped;
        enrollment.Notes = reason;
        enrollment.Course.AvailableSeats++;

        await _context.SaveChangesAsync();
        return (true, "Course dropped successfully");
    }

    public async Task<List<Course>> GetAvailableCoursesAsync(string studentId)
    {
        return await _context.Courses
            .Where(c => c.AvailableSeats > 0)
            .Include(c => c.PrerequisiteCourses)
            .ToListAsync();
    }

    public async Task<List<Enrollment>> GetCurrentEnrollmentsAsync(string studentId)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId && 
                       (e.Status == (EnrollmentStatus) EnrollmentStatus.Active || e.Status == (EnrollmentStatus) EnrollmentStatus.Pending))
            .ToListAsync();
    }

    public async Task<List<Enrollment>> GetEnrollmentHistoryAsync(string studentId)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync();
    }
}
