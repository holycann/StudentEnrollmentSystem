using Entityonlineform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

public class CoursesController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ApplicationDbContext _context;

    public CoursesController(
        IStudentService studentService,
        ApplicationDbContext context
    )
    {
        _studentService = studentService;
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> AvailableCourse(string sortOrder, string searchString, string filter)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Set up sorting
        var model = new AvailableCourseViewModel
        {
            CurrentStudent = student,
            CurrentSort = sortOrder,
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "",
            CodeSort = sortOrder == "code" ? "code_desc" : "code",
            CreditsSort = sortOrder == "credits" ? "credits_desc" : "credits",
            CurrentFilter = searchString ?? string.Empty,
            CurrentDepartmentFilter = filter ?? "all"
        };

        if (_context.Courses != null)
        {
            // Start with all active courses
            IQueryable<Course> coursesQuery = _context.Courses.Where(c => c.IsActive);

            // Apply department filter
            if (!string.IsNullOrEmpty(filter) && filter != "all")
            {
                if (filter == "CS")
                {
                    coursesQuery = coursesQuery.Where(c => c.CourseCode.StartsWith("CS"));
                }
                else if (filter == "MATH")
                {
                    coursesQuery = coursesQuery.Where(c => c.CourseCode.StartsWith("MATH"));
                }
                else if (filter == "BUS")
                {
                    coursesQuery = coursesQuery.Where(c => c.CourseCode.StartsWith("BUS"));
                }
                else if (filter == "SCI")
                {
                    coursesQuery = coursesQuery.Where(c =>
                        c.CourseCode.StartsWith("PHYS") ||
                        c.CourseCode.StartsWith("CHEM") ||
                        c.CourseCode.StartsWith("BIO"));
                }
                else if (filter == "HUM")
                {
                    coursesQuery = coursesQuery.Where(c =>
                        c.CourseCode.StartsWith("PHIL") ||
                        c.CourseCode.StartsWith("HIST"));
                }
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                coursesQuery = coursesQuery.Where(c =>
                    c.CourseName.Contains(searchString) ||
                    c.CourseCode.Contains(searchString) ||
                    (c.Description != null && c.Description.Contains(searchString)));
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "name_desc":
                    coursesQuery = coursesQuery.OrderByDescending(c => c.CourseName);
                    break;
                case "code":
                    coursesQuery = coursesQuery.OrderBy(c => c.CourseCode);
                    break;
                case "code_desc":
                    coursesQuery = coursesQuery.OrderByDescending(c => c.CourseCode);
                    break;
                case "credits":
                    coursesQuery = coursesQuery.OrderBy(c => c.Credits);
                    break;
                case "credits_desc":
                    coursesQuery = coursesQuery.OrderByDescending(c => c.Credits);
                    break;
                default:
                    coursesQuery = coursesQuery.OrderBy(c => c.CourseName);
                    break;
            }

            model.Courses = await coursesQuery.ToListAsync();

            model.Enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.StudentId)
                .ToListAsync();
        }

        return View(model);
    }

    [HttpPost("Courses/Enroll/{id}")]
    [Authorize]
    public async Task<IActionResult> OnPostEnrollAsync(int id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var CurrentStudent = await _studentService.GetStudentByUserIdAsync(userId);

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }

        // Check if student is already enrolled
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == CurrentStudent.StudentId && e.CourseId == id && e.Status == "Enrolled");

        if (existingEnrollment != null)
        {
            TempData["ErrorMessage"] = "You are already enrolled in this course.";
            return RedirectToAction("AvailableCourse");
        }

        // Check if course has capacity
        if (course.EnrolledCount >= course.Capacity)
        {
            TempData["ErrorMessage"] = "This course is full.";
            return View();
        }

        // Create enrollment
        var enrollment = new Enrollment
        {
            StudentId = CurrentStudent.StudentId,
            CourseId = id,
            EnrollmentDate = DateTime.Now,
            Status = "Enrolled"
        };

        _context.Enrollments.Add(enrollment);

        // Create add/drop record
        var addDropRecord = new AddDropRecord
        {
            StudentId = CurrentStudent.StudentId,
            CourseId = id,
            Action = "Add",
            ActionDate = DateTime.Now,
            Reason = "Online enrollment"
        };

        _context.AddDropRecords.Add(addDropRecord);

        // Update course enrolled count
        course.EnrolledCount++;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Successfully enrolled in the course.";
        return RedirectToAction("AvailableCourse");
    }

    [HttpPost("Courses/Drop/{id}")]
    [Authorize]
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
            return RedirectToAction("AvailableCourse");
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
        return RedirectToAction("AvailableCourse");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> History()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var CurrentStudent = await _studentService.GetStudentByUserIdAsync(userId);


        var addDropRecords = await _context.AddDropRecords
            .Include(a => a.Course)
            .Where(a => a.StudentId == CurrentStudent.StudentId)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync();

        var model = new AddDropRecordViewModel
        {
            AddDropRecords = addDropRecords,
            CurrentStudent = CurrentStudent
        };

        return View(model);
    }
}
