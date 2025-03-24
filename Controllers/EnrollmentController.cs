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
public class EnrollmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IEnrollmentService enrollmentService)
    {
        _context = context;
        _userManager = userManager;
        _enrollmentService = enrollmentService;
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        var enrollments = await _enrollmentService.GetEnrollmentHistoryAsync(user.Id);

        var currentEnrollments = enrollments
            .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
            .Select(e => new EnrollmentViewModel
            {
                Id = e.Id,
                CourseId = e.CourseId,
                StudentId = e.StudentId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                Notes = e.Notes,
                Course = new CourseViewModel
                {
                    Id = e.Course.Id,
                    CourseCode = e.Course.CourseCode,
                    Name = e.Course.Name,
                    Credits = e.Course.Credits,
                    Fee = e.Course.Fee
                }
            })
            .ToList();

        var pastEnrollments = enrollments
            .Where(e => e.Status == EnrollmentStatus.Completed || e.Status == EnrollmentStatus.Dropped)
            .Select(e => new EnrollmentViewModel
            {
                Id = e.Id,
                CourseId = e.CourseId,
                StudentId = e.StudentId,
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                Notes = e.Notes,
                Course = new CourseViewModel
                {
                    Id = e.Course.Id,
                    CourseCode = e.Course.CourseCode,
                    Name = e.Course.Name,
                    Credits = e.Course.Credits,
                    Fee = e.Course.Fee
                }
            })
            .ToList();

        var viewModel = new EnrollmentHistoryViewModel
        {
            CurrentEnrollments = currentEnrollments,
            PastEnrollments = pastEnrollments,
            TotalFees = enrollments.Where(e => e.Status != EnrollmentStatus.Dropped).Sum(e => e.Course.Fee),
            TotalCredits = enrollments.Where(e => e.Status == EnrollmentStatus.Completed).Sum(e => e.Course.Credits),
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Available()
    {
        var user = await _userManager.GetUserAsync(User);
        var availableCourses = await _enrollmentService.GetAvailableCoursesAsync(user.Id);
        var currentEnrollments = await _enrollmentService.GetCurrentEnrollmentsAsync(user.Id);

        var viewModels = new List<CourseEnrollmentViewModel>();

        foreach (var course in availableCourses)
        {
            var (canEnroll, missingPrerequisites) = await _enrollmentService.CanEnrollInCourseAsync(user.Id, course.Id);
            var isEnrolled = currentEnrollments.Any(e => e.CourseId == course.Id);

            viewModels.Add(new CourseEnrollmentViewModel
            {
                CourseId = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.Name,
                Schedule = course.CourseSchedules.Select(cs => new Models.ViewModels.TimeSlot
                {
                    Day = cs.DayOfWeek,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    Room = cs.Room,
                }).ToList().ToString(),
                Description = course.Description,
                CreditHours = course.Credits,
                Fee = course.Fee,
                IsAlreadyEnrolled = isEnrolled,
                PrerequisitesCodes = course.PrerequisiteCourses.Select(p => p.CourseCode).ToList(),
                MissingPrerequisites = missingPrerequisites
            });
        }

        return View(viewModels);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll([FromBody] EnrollmentRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);
        var (success, message) = await _enrollmentService.EnrollInCourseAsync(user.Id, model.CourseId);

        if (success)
        {
            return Json(new { success = true, message = "Enrollment successful" });
        }

        return BadRequest(new { success = false, message });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Drop([FromBody] DropCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.GetUserAsync(User);
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.Id == model.EnrollmentId && e.StudentId == user.Id);

        if (enrollment == null)
        {
            return NotFound(new { success = false, message = "Enrollment not found" });
        }

        (bool success, string message) = await _enrollmentService.DropCourseAsync(model.EnrollmentId, model.DropReason);

        if (success)
        {
            return Json(new { success = true, message = "Course dropped successfully" });
        }

        return BadRequest(new { success = false, message });
    }

    [HttpGet]
    public async Task<IActionResult> CheckPrerequisites(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        var (canEnroll, missingPrerequisites) = await _enrollmentService.CanEnrollInCourseAsync(user.Id, courseId);

        return Json(new
        {
            canEnroll,
            missingPrerequisites
        });
    }

    [HttpGet]
    public async Task<IActionResult> CheckScheduleConflict(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        var hasConflict = await _enrollmentService.HasScheduleConflictAsync(user.Id, courseId);

        return Json(new { hasConflict });
    }
}
