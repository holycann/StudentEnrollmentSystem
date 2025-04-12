using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

public class CoursesController : Controller
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICourseService _courseService;
    private readonly IStudentService _studentService;

    public CoursesController(
        IEnrollmentService enrollmentService,
        ICourseService courseService,
        IStudentService studentService
    )
    {
        _enrollmentService = enrollmentService;
        _courseService = courseService;
        _studentService = studentService;
    }

    public async Task<IActionResult> Index(
        string searchTerm,
        string sortOrder,
        int? creditFilter,
        decimal? maxFee,
        bool showAvailableOnly = false
    )
    {
        var courses = await _courseService.GetAllCoursesAsync();

        // // Apply filters
        // if (!string.IsNullOrEmpty(searchTerm))
        // {
        //     courses = courses.Where(c =>
        //         c.Name.Contains(searchTerm)
        //         || c.CourseCode.Contains(searchTerm)
        //         || c.Description.Contains(searchTerm)
        //     );
        // }

        // if (creditFilter.HasValue)
        // {
        //     courses = courses.Where(c => c.Credits == creditFilter);
        // }

        // if (maxFee.HasValue)
        // {
        //     courses = courses.Where(c => c.Fee <= maxFee);
        // }

        // if (showAvailableOnly)
        // {
        //     courses = courses.Where(c => c.AvailableSeats > 0);
        // }

        // Apply sorting
        // courses = sortOrder switch
        // {
        //     "name_desc" => courses.OrderByDescending(c => c.Name),
        //     "code" => courses.OrderBy(c => c.CourseCode),
        //     "code_desc" => courses.OrderByDescending(c => c.CourseCode),
        //     "credits" => courses.OrderBy(c => c.Credits),
        //     "credits_desc" => courses.OrderByDescending(c => c.Credits),
        //     "fee" => courses.OrderBy(c => c.FeePerCredit),
        //     "fee_desc" => courses.OrderByDescending(c => c.FeePerCredit),
        //     _ => courses.OrderBy(c => c.Name),
        // };

        // var courses = await coursesQuery
        //     .Select(c => new CourseViewModel
        //     {
        //         Id = c.Id,
        //         CourseCode = c.CourseCode,
        //         Name = c.Name,
        //         Description = c.Description,
        //         Credits = c.Credits,
        //         MaxSeats = c.MaxSeats,
        //         AvailableSeats = c.AvailableSeats,
        //         Fee = c.Fee,
        //         Prerequisites = c.Prerequisites,
        //     })
        //     .ToListAsync();

        var viewModel = new CourseListViewModel
        {
            // Courses = courses,
            SearchTerm = searchTerm,
            SortOrder = sortOrder,
            CreditFilter = creditFilter,
            MaxFee = maxFee,
            ShowAvailableOnly = showAvailableOnly,
        };

        return View(viewModel);
    }

    // public async Task<IActionResult> Details(int? id)
    // {
    //     if (id == null)
    //     {
    //         return NotFound();
    //     }

    //     var course = await _courseService.GetCourseByIdAsync(id.Value);
    //     if (course == null)
    //     {
    //         return NotFound();
    //     }

    //     var viewModel = new CourseViewModel
    //     {
    //         Id = course.Id,
    //         CourseCode = course.CourseCode,
    //         Name = course.Name,
    //         Description = course.Description,
    //         Credits = course.Credits,
    //         // MaxSeats = course.MaxSeats,
    //         // AvailableSeats = course.AvailableSeats,
    //         Fee = course.FeePerCredit,
    //         Prerequisites = course.PrerequisiteCourses.Select(c => c.Course.CourseCode).ToList(),
    //         PrerequisiteCourseIds = course.PrerequisiteCourses.Select(c => c.Id).ToList(),
    //     };

    //     return View(viewModel);
    // }

    [Authorize]
    public async Task<IActionResult> Enroll(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _courseService.GetCourseByIdAsync(id.Value);

        if (course == null)
        {
            return NotFound();
        }

        var userId = HttpContext.Session.GetString("UserId");

        var student = await _studentService.GetStudentByUserIdAsync(userId);
        var studentEnrollments = await _enrollmentService.GetEnrollmentsByStudentIdAsync(
            student.StudentId
        );

        // Check prerequisites
        var missingPrerequisites = new List<string>();
        foreach (var prereq in course.PrerequisiteCourses)
        {
            if (
                !studentEnrollments.Any(e =>
                    e.EnrollmentCourses.Any(ec => ec.CourseId == prereq.Id)
                )
            )
            {
                missingPrerequisites.Add($"{prereq.Course.CourseCode} - {prereq.Course.Name}");
            }
        }

        var viewModel = new EnrollmentRequestViewModel
        {
            CourseId = course.Id,
            Course = new CourseViewModel
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Name = course.Name,
                Description = course.Description,
                Credits = course.Credits,
                Fee = course.FeePerCredit,
                // AvailableSeats = course.MaxSeats - course.AvailableSeats,
            },
            HasPrerequisites = !missingPrerequisites.Any(),
            MissingPrerequisites = missingPrerequisites,
            TotalFee = course.Credits * course.FeePerCredit,
        };

        return View(viewModel);
    }

    // [HttpPost]
    // [Authorize]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Enroll(int id)
    // {
    //     var course = await _courseService.GetCourseByIdAsync(id);
    //     if (course == null)
    //     {
    //         return NotFound();
    //     }

    //     var student = await _studentService.GetStudentByUserIdAsync(
    //         HttpContext.Session.GetString("UserId")
    //     );

    //     // Check if already enrolled
    //     var existingEnrollment = await _enrollmentService
    //         .GetEnrollmentsByStudentIdAsync(student.StudentId)
    //         .AnyAsync(e =>
    //             e.CourseId == id
    //             && e.StudentId == student.StudentId
    //             && e.Status != EnrollmentStatus.Dropped
    //         );

    //     if (existingEnrollment)
    //     {
    //         TempData["Error"] = "You are already enrolled in this course.";
    //         return RedirectToAction(nameof(Details), new { id });
    //     }

    //     var enrollment = new Enrollment
    //     {
    //         StudentId = student.StudentId,
    //         EnrollmentDate = DateTime.UtcNow,
    //         Status = EnrollmentStatus.Pending,
    //         TotalCredits = course.Credits,
    //     };

    //     await _enrollmentService.EnrollInCourseAsync(student.StudentId, course.Id);

    //     TempData["Success"] = "You have successfully enrolled in this course.";
    //     return RedirectToAction(nameof(Index), "Enrollment");
    // }
}
