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
public class EnquiryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEnrollmentService _enrollmentService;

    public EnquiryController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IEnrollmentService enrollmentService)
    {
        _context = context;
        _userManager = userManager;
        _enrollmentService = enrollmentService;
    }

    // GET: /Enquiry/Contact
    public IActionResult Contact()
    {
        return View();
    }

    // POST: /Enquiry/Contact
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(EnquiryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var enquiry = new Enquiry
        {
            Subject = model.Subject,
            Message = model.Message,
            SubmissionDate = DateTime.UtcNow,
            Status = EnquiryStatus.Pending
        };

        _context.Enquiries.Add(enquiry);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Your enquiry has been submitted successfully.";
        return RedirectToAction(nameof(History));
    }

    // GET: /Enquiry/History
    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        var enquiries = await _context.Enquiries
            .Where(e => e.Student.StudentId == user.StudentId)
            .OrderByDescending(e => e.SubmissionDate)
            .Select(e => new EnquiryViewModel
            {
                Id = e.Id,
                Subject = e.Subject,
                Message = e.Message,
                Status = e.Status,
                Response = e.Response,
                CreatedAt = e.SubmissionDate
            })
            .ToListAsync();

        return View(enquiries);
    }

    // GET: /Enquiry/Timetable
    public async Task<IActionResult> Timetable()
    {
        var user = await _userManager.GetUserAsync(User);
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == user.Id && e.Status == EnrollmentStatus.Active)
            .ToListAsync();

        var schedule = new Dictionary<string, List<Models.ViewModels.TimeSlot>>();
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            schedule[day.ToString()] = new List<Models.ViewModels.TimeSlot>();
        }

        var courseSchedules = new List<CourseScheduleViewModel>();
        foreach (var enrollment in enrollments)     
        {

            var timeSlots = await _context.CourseSchedules
                .Where(cs => cs.CourseId == enrollment.CourseId)
                .Select(cs => new Models.ViewModels.TimeSlot
                {
                    Day = cs.DayOfWeek,
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    Room = cs.Room
                })
                .ToListAsync();

            courseSchedules.Add(new CourseScheduleViewModel
            {
                CourseId = enrollment.CourseId,
                CourseCode = enrollment.Course.CourseCode,
                CourseName = enrollment.Course.Name,
                TimeSlots = timeSlots
            });

            foreach (var slot in timeSlots)
            {
                schedule[slot.Day.ToString()].Add(slot);
            }
        }

        var viewModel = new TimetableViewModel
        {
            Courses = courseSchedules,
            Schedule = schedule
        };

        return View(viewModel);
    }

    // GET: /Enquiry/Evaluate/{courseId}
    public async Task<IActionResult> Evaluate(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == user.Id);

        if (enrollment == null)
        {
            return NotFound();
        }

        var existingEvaluation = await _context.TeachingEvaluations
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == user.Id);

        if (existingEvaluation != null)
        {
            TempData["ErrorMessage"] = "You have already submitted an evaluation for this course.";
            return RedirectToAction("History", "Enrollment");
        }

        var viewModel = new TeachingEvaluationViewModel
        {
            CourseId = courseId,
            InstructorName = enrollment.InstructorName
        };

        return View(viewModel);
    }

    // POST: /Enquiry/Evaluate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Evaluate(TeachingEvaluationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var evaluation = new TeachingEvaluation
        {
            CourseId = model.CourseId,
            StudentId = user.Id,
            InstructorName = model.InstructorName,
            TeachingEffectiveness = model.TeachingQuality,
            LearningMaterials = model.MaterialQuality,
            Feedback = model.AssignmentFeedback,
            Engagement = model.Engagement,
            AdditionalComments = model.Comments,
            SubmittedAt = DateTime.UtcNow
        };

        _context.TeachingEvaluations.Add(evaluation);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Thank you for submitting your evaluation.";
        return RedirectToAction("History", "Enrollment");
    }

    // GET: /Enquiry/CheckScheduleConflict
    [HttpGet]
    public async Task<IActionResult> CheckScheduleConflict(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        var hasConflict = await _enrollmentService.HasScheduleConflictAsync(user.Id, courseId);

        return Json(new { hasConflict });
    }
}
