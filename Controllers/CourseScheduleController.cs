using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class CourseScheduleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CourseScheduleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: CourseSchedule
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var schedules = await _context.Enrollments
            .Where(e => e.StudentId == user.Id)
            .Include(e => e.Course)
                .ThenInclude(c => c.CourseSchedules)
            .SelectMany(e => e.Course.CourseSchedules.Select(cs => new CourseScheduleViewModel
            {
                CourseId = cs.CourseId,
                CourseCode = e.Course.CourseCode,
                CourseName = e.Course.Name,
                DayAndTime = cs.DayOfWeek.ToString() + " " + cs.StartTime.ToString("hh:mm tt") + " - " + cs.EndTime.ToString("hh:mm tt"),
                Room = cs.Room,
                InstructorName = cs.InstructorName,
                Notes = cs.Notes
            }))
            .OrderBy(cs => cs.DayAndTime)
            .ToListAsync();

        return View(schedules);
    }

    // GET: CourseSchedule/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var schedule = await _context.CourseSchedules
            .Include(cs => cs.Course)
            .Where(cs => cs.Id == id)
            .Select(cs => new CourseScheduleViewModel
            {
                CourseId = cs.CourseId,
                CourseCode = cs.Course.CourseCode,
                CourseName = cs.Course.Name,
                DayAndTime = cs.DayOfWeek.ToString() + " " + cs.StartTime.ToString("hh:mm tt") + " - " + cs.EndTime.ToString("hh:mm tt"),
                Room = cs.Room,
                InstructorName = cs.InstructorName,
                Notes = cs.Notes
            })
            .FirstOrDefaultAsync();

        if (schedule == null)
        {
            return NotFound();
        }

        // Verify that the student is enrolled in this course
        var isEnrolled = await _context.Enrollments
            .AnyAsync(e => e.StudentId == user.Id && e.CourseId == schedule.CourseId);

        if (!isEnrolled)
        {
            return Forbid();
        }

        return View(schedule);
    }
}
