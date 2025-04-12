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
    private readonly UserManager<User> _userManager;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentService _studentService;

    public EnrollmentController(
        UserManager<User> userManager,
        IEnrollmentService enrollmentService,
        IStudentService studentService
    )
    {
        _userManager = userManager;
        _enrollmentService = enrollmentService;
        _studentService = studentService;
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        var enrollments = await _enrollmentService.GetEnrollmentHistoryAsync(user.Id);

        var currentEnrollments = enrollments
            .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
            .Select(e => new EnrollmentViewModel
            {
                EnrollmentCourses = e
                    .EnrollmentCourses.Select(ec => new EnrollmentCourseViewModel
                    {
                        EnrollmentId = e.Id,
                        Enrollment = new EnrollmentViewModel
                        {
                            EnrollmentDate = e.EnrollmentDate,
                            Status = e.Status,
                        },
                        CourseId = ec.CourseId,
                        Course = new CourseViewModel
                        {
                            Id = ec.Course.Id,
                            CourseCode = ec.Course.CourseCode,
                            Name = ec.Course.Name,
                            Credits = ec.Course.Credits,
                            Fee = ec.Course.FeePerCredit * ec.Course.Credits,
                        },
                    })
                    .ToList(),
            })
            .ToList();

        var pastEnrollments = enrollments
            .Where(e =>
                e.Status == EnrollmentStatus.Completed || e.Status == EnrollmentStatus.Dropped
            )
            .Select(e => new EnrollmentViewModel
            {
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                EnrollmentCourses = e
                    .EnrollmentCourses.Select(ec => new EnrollmentCourseViewModel
                    {
                        EnrollmentId = e.Id,
                        Enrollment = new EnrollmentViewModel
                        {
                            EnrollmentDate = e.EnrollmentDate,
                            Status = e.Status,
                        },
                        CourseId = ec.CourseId,
                        Course = new CourseViewModel
                        {
                            Id = ec.Course.Id,
                            CourseCode = ec.Course.CourseCode,
                            Name = ec.Course.Name,
                            Credits = ec.Course.Credits,
                            Fee = ec.Course.FeePerCredit * ec.Course.Credits,
                        },
                    })
                    .ToList(),
            })
            .ToList();

        var viewModel = new EnrollmentHistoryViewModel
        {
            CurrentEnrollments = currentEnrollments,
            PastEnrollments = pastEnrollments,
            TotalFees = enrollments
                .Where(e => e.Status != EnrollmentStatus.Dropped)
                .Sum(e =>
                    e.EnrollmentCourses.Sum(ec => ec.Course.FeePerCredit * ec.Course.Credits)
                ),
            TotalCredits = enrollments
                .Where(e => e.Status == EnrollmentStatus.Completed)
                .Sum(e => e.EnrollmentCourses.Sum(ec => ec.Course.Credits)),
        };

        return View(viewModel);
    }

    // [HttpGet]
    // public async Task<IActionResult> Available()
    // {
    //     var user = await _userManager.GetUserAsync(User);
    //     var availableCourses = await _enrollmentService.GetAvailableCoursesAsync(user.Id);
    //     var currentEnrollments = await _enrollmentService.GetCurrentEnrollmentsAsync(user.Id);

    //     var viewModels = new List<CourseEnrollmentViewModel>();

    //     foreach (var course in availableCourses)
    //     {
    //         var(bool canEnroll, List<string> missingPrerequisites) = await _enrollmentService.CanEnrollInCourseAsync(
    //             user.Id,
    //             course.Id
    //         );
    //         // var isEnrolled = currentEnrollments.Any(e => e.CourseId == course.Id);

    //         viewModels.Add(
    //             new CourseEnrollmentViewModel
    //             {
    //                 CourseId = course.Id,
    //                 CourseCode = course.CourseCode,
    //                 CourseName = course.Name,
    //                 Schedule = course
    //                     .CourseSchedules.Select(cs => new Models.ViewModels.TimeSlot
    //                     {
    //                         Day = cs.DayOfWeek,
    //                         StartTime = cs.StartTime,
    //                         EndTime = cs.EndTime,
    //                         Room = cs.Room,
    //                     })
    //                     .ToList()
    //                     .ToString(),
    //                 Description = course.Description,
    //                 CreditHours = course.Credits,
    //                 Fee = course.FeePerCredit * course.Credits,
    //                 // IsAlreadyEnrolled = isEnrolled,
    //                 PrerequisitesCodes = course
    //                     .PrerequisiteCourses.Select(p => p.Course.CourseCode)
    //                     .ToList(),
    //                 MissingPrerequisites = missingPrerequisites,
    //                 CanEnroll = canEnroll,
    //             }
    //         );
    //     }

    //     return View(viewModels);
    // }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Enroll([FromBody] EnrollmentRequestViewModel model)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }

    //     var student = await _studentService.GetStudentByUserIdAsync(
    //         HttpContext.Session.GetString("UserId")
    //     );
    //     var(bool success, string message) = await _enrollmentService.EnrollInCourseAsync(
    //         student.StudentId,
    //         model.CourseId
    //     );

    //     if (success)
    //     {
    //         return Json(new { success = true, message = "Enrollment successful" });
    //     }

    //     return BadRequest(new { success = false, message });
    // }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Drop(DropCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var student = await _studentService.GetStudentByUserIdAsync(
            HttpContext.Session.GetString("UserId")
        );
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(model.EnrollmentId);

        if (enrollment == null)
        {
            return NotFound(new { success = false, message = "Enrollment not found" });
        }

        (bool success, string message) = await _enrollmentService.DropCourseAsync(
            model.EnrollmentId,
            model.DropReason
        );

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
        var (canEnroll, missingPrerequisites) = await _enrollmentService.CanEnrollInCourseAsync(
    user.Id,
    courseId
);

        return Json(new { canEnroll, missingPrerequisites });
    }

    // [HttpGet]
    // public async Task<IActionResult> CheckScheduleConflict(int courseId)
    // {
    //     var user = await _userManager.GetUserAsync(User);
    //     var hasConflict = await _enrollmentService.HasScheduleConflictAsync(user.Id, courseId);

    //     return Json(new { hasConflict });
    // }
}
