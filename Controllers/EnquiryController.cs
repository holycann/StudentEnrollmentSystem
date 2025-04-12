using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class EnquiryController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IEnrollmentService _enrollmentService;
    private readonly ILogger<EnquiryController> _logger;

    public EnquiryController(
        UserManager<User> userManager,
        IEnrollmentService enrollmentService,
        ILogger<EnquiryController> logger
    )
    {
        _userManager = userManager;
        _enrollmentService = enrollmentService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        var Enquery = new Enquiry();

        return View(Enquery);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Index(Enquiry model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        // In a real application, you would save the enquiry to a database
        // For now, we'll just redirect to a success page
        TempData["SuccessMessage"] = "Your enquiry has been submitted successfully. We will get back to you soon.";

        return RedirectToAction("Success", "Enquiry");
    }

    [HttpGet]
    [Authorize]
    public IActionResult ContactUs()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public IActionResult ContactUs(ContactMessage model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        // In a real application, you would save the contact message to a database
        // and potentially send an email notification

        TempData["SuccessMessage"] = "Your message has been sent successfully. We will get back to you soon.";

        return RedirectToAction("ContactSuccess", "Enquiry");
    }

    [HttpGet]
    [Authorize]
    public IActionResult ContactSuccess()
    {
        // If there's no success message in TempData, redirect to the contact form
        if (TempData["SuccessMessage"] == null)
        {
            RedirectToAction("ContactUs", "Enquiry");
        }

        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Success()
    {
        if (TempData["SuccessMessage"] == null)
        {
            RedirectToAction("Index", "Enquiry");
        }

        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult StudentEvaluation()
    {
        // Initialize the evaluation object
        var model = new StudentEvaluationViewModels
        {
            StudentEvaluation = new Models.StudentEvaluation(),

            // Populate available courses (in a real app, this would come from a database)
            AvailableCourses = new Dictionary<string, string>
            {
                { "CS101", "Introduction to Computer Science" },
                { "CS201", "Data Structures and Algorithms" },
                { "CS301", "Database Systems" },
                { "CS401", "Software Engineering" },
                { "MATH101", "Calculus I" },
                { "MATH201", "Linear Algebra" }
            },

            // Populate course data with additional information
            CourseData = new Dictionary<string, CourseInfo>
            {
                { "CS101", new CourseInfo { Name = "Introduction to Computer Science", Instructor = "Dr. John Smith" } },
                { "CS201", new CourseInfo { Name = "Data Structures and Algorithms", Instructor = "Dr. Jane Doe" } },
                { "CS301", new CourseInfo { Name = "Database Systems", Instructor = "Prof. Robert Johnson" } },
                { "CS401", new CourseInfo { Name = "Software Engineering", Instructor = "Dr. Emily Chen" } },
                { "MATH101", new CourseInfo { Name = "Calculus I", Instructor = "Prof. Michael Brown" } },
                { "MATH201", new CourseInfo { Name = "Linear Algebra", Instructor = "Dr. Sarah Wilson" } }
            },
        };


        return View(model);
    }

    [HttpPost]
    [Authorize]
    public IActionResult StudentEvaluation(StudentEvaluationViewModels model)
    {
        if (!ModelState.IsValid)
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            Console.WriteLine("Model in StudentEvaluation: " + json);

            // Cetak error per field
            foreach (var entry in ModelState)
            {
                var field = entry.Key;
                var errors = entry.Value.Errors;

                foreach (var error in errors)
                {
                    Console.WriteLine($"Error in field '{field}': {error.ErrorMessage}");
                }
            }

            // Re-populate the dropdown data if validation fails
            return View();
        }


        // In a real application, save the evaluation to a database
        _logger.LogInformation("Student evaluation submitted for course: {CourseCode}", model.StudentEvaluation.CourseCode);

        // Redirect to a thank you page or show a success message
        TempData["SuccessMessage"] = "Thank you for your feedback! Your evaluation has been submitted.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public IActionResult TimetableMatching()
    {
        // Initialize the evaluation object
        var model = new TimetableMatchingViewModel
        {
            AvailableCourses = new List<string>
            {
                "CS101: Introduction to Programming",
                "CS201: Data Structures and Algorithms",
                "CS301: Database Systems",
                "CS401: Software Engineering",
                "MATH101: Calculus I",
                "MATH201: Linear Algebra",
                "ENG101: English Composition",
                "PHYS101: Physics I",
                "BIO101: Biology I",
                "CHEM101: Chemistry I"
            },
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public IActionResult TimetableMatching(TimetableMatchingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        // Process the selected courses
        model.TimetableRequest.PreferredCourses = model.SelectedCourses;

        // Process the unavailable times
        foreach (var timeSlot in model.UnavailableTimes)
        {
            var parts = timeSlot.Split('-');
            if (parts.Length == 2)
            {
                if (Enum.TryParse<DayOfWeek>(parts[0], out var day))
                {
                    var timeOfDay = parts[1];
                    var unavailableTime = new UnavailableTime { DayOfWeek = day };

                    // Set the time range based on the time of day
                    switch (timeOfDay)
                    {
                        case "Morning":
                            unavailableTime.StartTime = new TimeSpan(8, 0, 0);
                            unavailableTime.EndTime = new TimeSpan(12, 0, 0);
                            break;
                        case "Afternoon":
                            unavailableTime.StartTime = new TimeSpan(12, 0, 0);
                            unavailableTime.EndTime = new TimeSpan(17, 0, 0);
                            break;
                        case "Evening":
                            unavailableTime.StartTime = new TimeSpan(17, 0, 0);
                            unavailableTime.EndTime = new TimeSpan(21, 0, 0);
                            break;
                    }

                    model.TimetableRequest.UnavailableTimes.Add(unavailableTime);
                }
            }
        }

        // In a real application, you would save the timetable request to a database
        // and process it to generate a matching timetable

        TempData["SuccessMessage"] = "Your timetable matching request has been submitted successfully. We will process your request and send you an optimized timetable within 2 business days.";

        return RedirectToAction("SuccessTimetableMatching", "Enquiry");
    }

    [HttpGet]
    [Authorize]
    public IActionResult SuccessTimetableMatching()
    {
        // If there's no success message in TempData, redirect to the timetable matching form
        if (TempData["SuccessMessage"] == null)
        {
            RedirectToAction("TimetableMatching", "Enquiry");
        }

        return View();
    }

}
