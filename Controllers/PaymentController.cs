using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IStudentService _studentService;
    private readonly IUserService _userService;
    private readonly IProgramService _programService;
    private readonly ISemesterService _semesterService;
    private readonly ApplicationDbContext _context;

    public PaymentController(
        IPaymentService paymentService,
        IStudentService studentService,
        IUserService userService,
        IProgramService programService,
        ISemesterService semesterService,
        ApplicationDbContext context
    )
    {
        _paymentService = paymentService;
        _studentService = studentService;
        _userService = userService;
        _programService = programService;
        _semesterService = semesterService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> MakePayment()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student.StudentId == null)
        {
            return NotFound("Student Id not found");
        }

        var nextSemesterPayment = await _paymentService.GetPaymentsBySemesterIdAsync(
            student.StudentId,
            student.Semester.SemesterNumber + 1
        );

        if (nextSemesterPayment != null && nextSemesterPayment.Status == PaymentStatus.Completed)
        {
            return NotFound("Payment already completed");
        }

        var viewModel = new MakePaymentViewModel
        {
            StudentId = student.StudentId,
            SemesterId = student.Semester.Id + 1,
            ProgramName = student.ProgramStudy.Name,
            SemesterName = student.Semester.Name,
            StudentViewModel = new StudentViewModel
            {
                StudentId = student.StudentId,
                BankAccountNumber = student.BankAccountNumber,
                BankName = student.BankName,
            },
        };

        viewModel.Courses = await _context.Courses
            .Where(c => c.SemesterId == student.Semester.Id && c.ProgramStudyId == student.ProgramStudy.Id)
            .ToListAsync();

        viewModel.Amount = viewModel.Courses.Sum(c => c.Fee * c.Credits);

        Console.WriteLine($"Amount: {viewModel.Amount}");

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(MakePaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("MakePayment", model);
        }

        var student = await _studentService.GetStudentByIdAsync(model.StudentId);

        model.StudentViewModel = new StudentViewModel
        {
            StudentId = student.StudentId,
            BankAccountNumber = student.BankAccountNumber,
            BankName = student.BankName,
        };

        var semester = await _semesterService.GetSemesterByIdAsync(model.SemesterId);

        if (semester == null)
        {
            var addSemester = new Semester
            {
                Name = "Semester " + (student.Semester.SemesterNumber + 1),
                SemesterNumber = student.Semester.SemesterNumber + 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            await _semesterService.AddSemesterAsync(addSemester);
        }

        var (payment, message) = await _paymentService.ProcessPaymentAsync(model);
        if (payment != null)
        {
            if (payment.PaymentMethod != PaymentMethod.Cards)
            {
                return RedirectToAction(
                    "WaitingPayment",
                    new WaitingPaymentViewModel { PaymentId = payment.Id, MakePaymentViewModel = model }
                );
            }
            else
            {
                return RedirectToAction("PaymentSuccess", new { paymentId = payment.Id });
            }
        }

        ModelState.AddModelError("", message);
        return View("MakePayment", model);
    }

    [HttpGet]
    public async Task<IActionResult> WaitingPayment(WaitingPaymentViewModel model)
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payment = await _paymentService.GetPaymentByIdAsync(model.PaymentId);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        model.StudentName = user.Fullname;
        model.ExpirationDate = payment.Semester.StartDate.AddDays(-14);

        model.MakePaymentViewModel = new MakePaymentViewModel
        {
            StudentId = payment.StudentId,
            SemesterId = payment.SemesterId,
            ProgramName = payment.Student.ProgramStudy.Name,
            SemesterName = payment.Semester.Name,
            Amount = payment.Amount,
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> PaymentSuccess(int paymentId)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payment = await _paymentService.GetPaymentByIdAsync(paymentId);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        payment.Status = PaymentStatus.Completed;
        payment.PaymentDate = DateTime.Now;

        var (success, message) = await _paymentService.UpdatePaymentAsync(payment);
        if (success)
        {
            return View();
        }
        else
        {
            ModelState.AddModelError("", message);
            return View();
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payment = await _paymentService.GetPaymentByIdAsync(id);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var viewModel = new PaymentDetailsViewModel
        {
            StudentName = user.Fullname,
            StudentId = payment.StudentId,
            Email = user.Email,
            ProgramName = payment.Student.ProgramStudy.Name,
            SemesterName = payment.Semester.Name,
            PaymentId = payment.Id,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod.ToString(),
            PaymentStatus = payment.Status,
            Amount = payment.Amount,
        };
        return View(viewModel);
    }

    public async Task<IActionResult> History()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payments = await _paymentService.GetPaymentsByStudentIdAsync(student.StudentId);

        if (payments == null)
        {
            return NotFound("Payments not found");
        }

        var notPendingPayments = payments.Where(p => p.Status != PaymentStatus.Pending).ToList();

        if (notPendingPayments == null)
        {
            return NotFound("Not pending payments not found");
        }

        var viewModel = notPendingPayments
            .Select(p => new PaymentHistoryViewModel
            {
                PaymentId = p.Id,
                SemesterName = p.Semester.Name,
                PaymentMethod = p.PaymentMethod.ToString(),
                PaymentDate = p.PaymentDate,
                Status = p.Status.ToString(),
                Amount = p.Amount,
            })
            .ToList();

        return View(viewModel);
    }

    public async Task<IActionResult> Invoice()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payments = await _paymentService.GetPaymentsByStudentIdAsync(student.StudentId);

        var invoice = payments.Where(p => p.Status == PaymentStatus.Pending).ToList();

        var viewModel = invoice
            .Select(p => new PaymentInvoiceViewModel
            {
                PaymentId = p.Id,
                ProgramName = p.Student.ProgramStudy.Name,
                SemesterName = p.Semester.Name,
                PaymentMethod = p.PaymentMethod.ToString(),
                ExpirationDate = p.Semester.StartDate.AddDays(-14),
                Amount = p.Amount,
            })
            .ToList();

        return View(viewModel);
    }

    public async Task<IActionResult> DetailInvoice(int id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payment = await _paymentService.GetPaymentByIdAsync(id);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var viewModel = new PaymentInvoiceDetailsViewModel
        {
            StudentName = user.Fullname,
            StudentId = payment.StudentId,
            Email = user.Email,
            ProgramName = payment.Student.ProgramStudy.Name,
            SemesterName = payment.Semester.Name,
            PaymentId = payment.Id,
            PaymentMethod = payment.PaymentMethod.ToString(),
            ExpirationDate = payment.Semester.StartDate.AddDays(-14),
            Amount = payment.Amount,
        };
        return View(viewModel);
    }

    public async Task<IActionResult> Adjustment(int id)
    {
        var userId = HttpContext.Session.GetString("UserId");

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var payment = await _paymentService.GetPaymentByIdAsync(id);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var programs = await _programService.GetAllProgramsAsync();

        if (programs == null)
        {
            return NotFound("Programs not found");
        }

        var courses = await _context.Courses.ToListAsync();

        if (courses == null)
        {
            return NotFound("Courses not found");
        }

        var semesters = await _semesterService.GetAllSemestersAsync();

        if (semesters == null)
        {
            return NotFound("Semesters not found");
        }

        var viewModel = new PaymentAdjustmentViewModel
        {
            PaymentId = payment.Id,
            StudentId = payment.StudentId,
            SemesterId = payment.SemesterId,
            ProgramId = payment.Student.ProgramId,
            StudentName = user.Fullname,
            Email = user.Email,
            ProgramName = payment.Student.ProgramStudy.Name,
            SemesterName = payment.Semester.Name,
            PaymentMethod = payment.PaymentMethod,
            Courses = courses
                .Select(c => new SelectListItem { Value = c.CourseId.ToString(), Text = c.CourseName })
                .ToList(),
            Programs = programs
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList(),
            Semesters = semesters
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList(),
            PaymentMethods = Enum.GetValues(typeof(PaymentMethod))
                .Cast<PaymentMethod>()
                .Select(pm => new SelectListItem { Value = pm.ToString(), Text = pm.ToString() })
                .ToList(),
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessAdjustment(PaymentAdjustmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Adjustment", model);
        }

        var payment = await _paymentService.GetPaymentByIdAsync(model.PaymentId);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var courses = await _context.Courses
            .Where(c => model.SelectedCourses.Contains(c.CourseId))
            .ToListAsync();

        var amountPaid = courses.Sum(c => c.Fee * c.Credits);

        if (amountPaid != payment.Amount || model.PaymentMethod != payment.PaymentMethod)
        {
            var (success, message) = await _paymentService.ProcessAdjustmentAsync(
                payment.Id,
                amountPaid,
                model.PaymentMethod
            );
            if (success)
            {
                return RedirectToAction("Invoice");
            }
            else
            {
                ModelState.AddModelError("", message);
                return View(model);
            }
        }

        ModelState.AddModelError("", "No changes to update");
        return View(model);
    }
}
