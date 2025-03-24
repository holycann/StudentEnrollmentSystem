using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string searchTerm, string sortOrder, int? creditFilter, decimal? maxFee, bool showAvailableOnly = false)
    {
        var coursesQuery = _context.Courses.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            coursesQuery = coursesQuery.Where(c => 
                c.Name.Contains(searchTerm) || 
                c.CourseCode.Contains(searchTerm) ||
                c.Description.Contains(searchTerm));
        }

        if (creditFilter.HasValue)
        {
            coursesQuery = coursesQuery.Where(c => c.Credits == creditFilter);
        }

        if (maxFee.HasValue)
        {
            coursesQuery = coursesQuery.Where(c => c.Fee <= maxFee);
        }

        if (showAvailableOnly)
        {
            coursesQuery = coursesQuery.Where(c => c.AvailableSeats > 0);
        }

        // Apply sorting
        coursesQuery = sortOrder switch
        {
            "name_desc" => coursesQuery.OrderByDescending(c => c.Name),
            "code" => coursesQuery.OrderBy(c => c.CourseCode),
            "code_desc" => coursesQuery.OrderByDescending(c => c.CourseCode),
            "credits" => coursesQuery.OrderBy(c => c.Credits),
            "credits_desc" => coursesQuery.OrderByDescending(c => c.Credits),
            "fee" => coursesQuery.OrderBy(c => c.Fee),
            "fee_desc" => coursesQuery.OrderByDescending(c => c.Fee),
            _ => coursesQuery.OrderBy(c => c.Name)
        };

        var courses = await coursesQuery
            .Select(c => new CourseViewModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                MaxSeats = c.MaxSeats,
                AvailableSeats = c.AvailableSeats,
                Fee = c.Fee,
                Prerequisites = c.Prerequisites
            })
            .ToListAsync();

        var viewModel = new CourseListViewModel
        {
            Courses = courses,
            SearchTerm = searchTerm,
            SortOrder = sortOrder,
            CreditFilter = creditFilter,
            MaxFee = maxFee,
            ShowAvailableOnly = showAvailableOnly
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.PrerequisiteCourses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        var viewModel = new CourseViewModel
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            Name = course.Name,
            Description = course.Description,
            Credits = course.Credits,
            MaxSeats = course.MaxSeats,
            AvailableSeats = course.AvailableSeats,
            Fee = course.Fee,
            Prerequisites = course.Prerequisites,
            PrerequisiteCourseIds = course.PrerequisiteCourses.Select(c => c.Id).ToList()
        };

        return View(viewModel);
    }

    [Authorize]
    public async Task<IActionResult> Enroll(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.PrerequisiteCourses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        var userEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == user.Id)
            .ToListAsync();

        // Check prerequisites
        var missingPrerequisites = new List<string>();
        foreach (var prereq in course.PrerequisiteCourses)
        {
            if (!userEnrollments.Any(e => e.CourseId == prereq.Id && e.Status == EnrollmentStatus.Completed))
            {
                missingPrerequisites.Add($"{prereq.CourseCode} - {prereq.Name}");
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
                Fee = course.Fee,
                AvailableSeats = course.AvailableSeats
            },
            HasPrerequisites = !missingPrerequisites.Any(),
            MissingPrerequisites = missingPrerequisites,
            TotalFee = course.Fee
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        // Check if already enrolled
        var existingEnrollment = await _context.Enrollments
            .AnyAsync(e => e.CourseId == id && e.StudentId == user.Id && e.Status != EnrollmentStatus.Dropped);

        if (existingEnrollment)
        {
            TempData["Error"] = "You are already enrolled in this course.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Check available seats
        if (course.AvailableSeats <= 0)
        {
            TempData["Error"] = "This course is full.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var enrollment = new Enrollment
        {
            CourseId = id,
            StudentId = user.Id,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Pending
        };

        course.AvailableSeats--;

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "You have successfully enrolled in this course.";
        return RedirectToAction(nameof(Index), "Enrollment");
    }
}
