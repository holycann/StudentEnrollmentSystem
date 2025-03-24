using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class StatementsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEnrollmentService _enrollmentService;

    public StatementsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IEnrollmentService enrollmentService)
    {
        _context = context;
        _userManager = userManager;
        _enrollmentService = enrollmentService;
    }

    // GET: /Statements
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var statement = await GenerateStatement(user.Id);
        return View(statement);
    }

    // GET: /Statements/Download
    public async Task<IActionResult> Download()
    {
        var user = await _userManager.GetUserAsync(User);
        var statement = await GenerateStatement(user.Id);
        return View("Download", statement);
    }

    private async Task<StatementViewModel> GenerateStatement(string userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == userId)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => p.StudentId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        var courseSchedules = await _context.CourseSchedules
            .Where(cs => enrollments.Select(e => e.CourseId).Contains(cs.CourseId))
            .ToListAsync();

        var enrollmentStatements = enrollments.Select(e => new EnrollmentStatement
        {
            EnrollmentId = e.Id,
            CourseCode = e.Course.CourseCode,
            CourseName = e.Course.Name,
            Credits = e.Course.Credits,
            Fee = e.Course.Fee,
            EnrollmentDate = e.EnrollmentDate,
            Status = e.Status,
            Schedule = courseSchedules
                .Where(cs => cs.CourseId == e.CourseId)
                .Select(cs => new Models.ViewModels.TimeSlot
                {
                    Day = cs.DayOfWeek,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    Room = cs.Room,
                })
                .ToList()
        }).ToList();

        var paymentStatements = payments.Select(p => new PaymentStatement
        {
            PaymentId = p.Id,
            TransactionId = p.TransactionId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            Status = p.Status,
            PaymentMethod = p.PaymentMethod,
        }).ToList();

        var weeklySchedule = new Dictionary<DayOfWeek, List<Models.ViewModels.TimeSlot>>();
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            weeklySchedule[day] = courseSchedules
                .Where(cs => cs.DayOfWeek == day)
                .Select(cs => new Models.ViewModels.TimeSlot
                {
                    Day = cs.DayOfWeek,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    Room = cs.Room,
                })
                .ToList();
        }

        var registrationSummary = new RegistrationSummary
        {
            TotalCredits = enrollments.Sum(e => e.Course.Credits),
            TotalCourses = enrollments.Count,
            RegistrationDate = enrollments.Min(e => e.EnrollmentDate),
            AcademicTerm = "2025 Spring", // This should come from configuration or database
            EnrolledCourses = enrollments.Select(e => e.Course.CourseCode).ToList(),
            WeeklySchedule = weeklySchedule
        };

        return new StatementViewModel
        {
            StudentId = user.Id,
            StudentName = $"{user?.FirstName} {user?.LastName}",
            StudentEmail = user.Email,
            GeneratedDate = DateTime.UtcNow,
            TotalFees = enrollments.Sum(e => (decimal)e.TotalFee),
            TotalPaid = payments.Where(p => (PaymentStatus)p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
            Enrollments = enrollmentStatements,
            Payments = paymentStatements,
            Program = user.Program,
            Registration = registrationSummary
        };
    }
}
