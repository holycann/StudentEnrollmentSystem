using Entityonlineform.Models;
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
    private readonly IStudentService _studentService;
    private readonly ApplicationDbContext _context;

    public EnrollmentController(
        UserManager<User> userManager,
        IStudentService studentService,
        ApplicationDbContext context
    )
    {
        _userManager = userManager;
        _studentService = studentService;
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyEnrollment()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var CurrentStudent = await _studentService.GetStudentByUserIdAsync(userId);

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == CurrentStudent.StudentId && e.Status == "Enrolled")
            .ToListAsync();

        var model = new EnrollmentViewModel
        {
            CurrentStudent = CurrentStudent,
            Enrollments = enrollments
        };

        return View(model);
    }

    [HttpPost("Enrollment/Drop/{id}")]
    public async Task<IActionResult> OnPostDropAsync(int id)
    {
        var CurrentStudent = await _studentService.GetStudentByUserIdAsync(HttpContext.Session.GetString("UserId"));

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        // Find the enrollment
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == CurrentStudent.StudentId && e.CourseId == id && e.Status == "Enrolled");

        if (enrollment == null)
        {
            TempData["ErrorMessage"] = "You are not enrolled in this course.";
            return RedirectToAction("MyEnrollment", "Enrollment");
        }

        // Update enrollment status
        enrollment.Status = "Dropped";

        // Create add/drop record
        var addDropRecord = new AddDropRecord
        {
            StudentId = CurrentStudent.StudentId,
            CourseId = id,
            Action = "Drop",
            ActionDate = DateTime.Now,
            Reason = "Online drop"
        };

        _context.AddDropRecords.Add(addDropRecord);

        // Update course enrolled count
        course.EnrolledCount--;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Successfully dropped the course.";
        return RedirectToAction("MyEnrollment", "Enrollment");
    }
}
