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
    private readonly IEnrollmentService _enrollmentService;
    private readonly IUserService _userService;
    private readonly IProgramService _programService;
    private readonly ISemesterService _semesterService;
    private readonly ICourseService _courseService;

    public PaymentController(
        IPaymentService paymentService,
        IStudentService studentService,
        IUserService userService,
        IEnrollmentService enrollmentService,
        IProgramService programService,
        ISemesterService semesterService,
        ICourseService courseService
    )
    {
        _paymentService = paymentService;
        _studentService = studentService;
        _userService = userService;
        _enrollmentService = enrollmentService;
        _programService = programService;
        _semesterService = semesterService;
        _courseService = courseService;
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

        var nextEnrollmentSemester = await _enrollmentService.GetEnrollmentByNextSemesterAsync(
            student.StudentId,
            student.Semester.SemesterNumber
        );

        if (nextEnrollmentSemester.Status == EnrollmentStatus.Active)
        {
            return NotFound("Next semester is already active");
        }

        var viewModel = new MakePaymentViewModel
        {
            StudentId = student.StudentId,
            EnrollmentId = nextEnrollmentSemester.Id,
            ProgramName = nextEnrollmentSemester.Program.Name,
            SemesterName = nextEnrollmentSemester.Semester.Name,
            Amount = nextEnrollmentSemester.EnrollmentCourses.Sum(ec =>
                ec.Course.FeePerCredit * ec.Course.Credits
            ),
            Enrollment = new EnrollmentViewModel
            {
                Id = nextEnrollmentSemester.Id,
                Status = nextEnrollmentSemester.Status,
                TotalCredits = nextEnrollmentSemester.EnrollmentCourses.Sum(ec =>
                    ec.Course.Credits
                ),
                TotalFees = nextEnrollmentSemester.EnrollmentCourses.Sum(ec =>
                    ec.Course.FeePerCredit * ec.Course.Credits
                ),
                EnrollmentCourses = nextEnrollmentSemester
                    .EnrollmentCourses.Select(ec => new EnrollmentCourseViewModel
                    {
                        EnrollmentId = ec.EnrollmentId,
                        CourseId = ec.CourseId,
                        Course = new CourseViewModel
                        {
                            Id = ec.CourseId,
                            Name = ec.Course.Name,
                            Fee = ec.Course.FeePerCredit,
                            Credits = ec.Course.Credits,
                        },
                    })
                    .ToList(),
            },
            Student = new StudentViewModel
            {
                StudentId = student.StudentId,
                ProgramName = nextEnrollmentSemester.Program.Name,
                BankAccountNumber = student.BankAccountNumber,
                BankName = student.BankName,
            },
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(MakePaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            return View("MakePayment", model);
        }

        var student = await _studentService.GetStudentByIdAsync(model.StudentId);
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(model.EnrollmentId);

        model.Student = new StudentViewModel
        {
            StudentId = student.StudentId,
            BankAccountNumber = student.BankAccountNumber,
            BankName = student.BankName,
        };
        model.Enrollment = new EnrollmentViewModel
        {
            Id = enrollment.Id,
            Status = enrollment.Status,
            TotalCredits = enrollment.EnrollmentCourses.Sum(ec => ec.Course.Credits),
            TotalFees = enrollment.EnrollmentCourses.Sum(ec =>
                ec.Course.FeePerCredit * ec.Course.Credits
            ),
        };

        var (payment, message) = await _paymentService.ProcessPaymentAsync(model);
        if (payment != null)
        {
            if (payment.PaymentMethod != PaymentMethod.Cards)
            {
                return RedirectToAction(
                    "WaitingPayment",
                    new WaitingPaymentViewModel { PaymentId = payment.Id, Payment = model }
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
        model.ExpirationDate = payment.Enrollment.EnrollmentDate.AddDays(14);

        var json = JsonConvert.SerializeObject(payment, Formatting.Indented);

        model.Payment = new MakePaymentViewModel
        {
            StudentId = payment.StudentId,
            EnrollmentId = payment.EnrollmentId,
            ProgramName = payment.Enrollment.Program.Name,
            SemesterName = payment.Enrollment.Semester.Name,
            Amount = payment.Amount,
            Enrollment = new EnrollmentViewModel
            {
                Id = payment.EnrollmentId,
                Status = payment.Enrollment.Status,
                EnrollmentCourses = payment
                    .Enrollment.EnrollmentCourses.Select(ec => new EnrollmentCourseViewModel
                    {
                        EnrollmentId = ec.EnrollmentId,
                        CourseId = ec.CourseId,
                        Course = new CourseViewModel
                        {
                            Id = ec.CourseId,
                            Name = ec.Course.Name,
                            Fee = ec.Course.FeePerCredit,
                            Credits = ec.Course.Credits,
                        },
                    })
                    .ToList(),
            },
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

        var json = JsonConvert.SerializeObject(payment, Formatting.Indented);
        Console.WriteLine("Payment: " + json);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var viewModel = new PaymentDetailsViewModel
        {
            StudentName = user.Fullname,
            StudentId = payment.StudentId,
            Email = user.Email,
            EnrollmentId = payment.EnrollmentId,
            ProgramName = payment.Enrollment.Program.Name,
            SemesterName = payment.Enrollment.Semester.Name,
            EnrollmentDate = payment.Enrollment.EnrollmentDate,
            EnrollmentStatus = payment.Enrollment.Status,
            EnrollmentCourses = payment
                .Enrollment.EnrollmentCourses.Select(ec => new EnrollmentCourseViewModel
                {
                    EnrollmentId = ec.EnrollmentId,
                    CourseId = ec.CourseId,
                    Course = new CourseViewModel
                    {
                        Id = ec.CourseId,
                        Name = ec.Course.Name,
                        Fee = ec.Course.FeePerCredit,
                        Credits = ec.Course.Credits,
                    },
                })
                .ToList(),
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
                EnrollmentId = p.EnrollmentId,
                SemesterName = p.Enrollment.Semester.Name,
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
                EnrollmentId = p.EnrollmentId,
                ProgramName = p.Enrollment.Program.Name,
                SemesterName = p.Enrollment.Semester.Name,
                PaymentMethod = p.PaymentMethod.ToString(),
                ExpirationDate = p.Enrollment.EnrollmentDate.AddDays(14),
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

        var json = JsonConvert.SerializeObject(payment, Formatting.Indented);
        Console.WriteLine("Payment: " + json);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var viewModel = new PaymentInvoiceDetailsViewModel
        {
            StudentName = user.Fullname,
            StudentId = payment.StudentId,
            Email = user.Email,
            EnrollmentId = payment.EnrollmentId,
            ProgramName = payment.Enrollment.Program.Name,
            SemesterName = payment.Enrollment.Semester.Name,
            EnrollmentDate = payment.Enrollment.EnrollmentDate,
            EnrollmentCourses = payment
                .Enrollment.EnrollmentCourses.Select(ec => new EnrollmentCourseViewModel
                {
                    EnrollmentId = ec.EnrollmentId,
                    CourseId = ec.CourseId,
                    Course = new CourseViewModel
                    {
                        Id = ec.CourseId,
                        Name = ec.Course.Name,
                        Fee = ec.Course.FeePerCredit,
                        Credits = ec.Course.Credits,
                    },
                })
                .ToList(),
            PaymentId = payment.Id,
            PaymentMethod = payment.PaymentMethod.ToString(),
            ExpirationDate = payment.Enrollment.EnrollmentDate.AddDays(14),
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

        var courses = await _courseService.GetCoursesByProgramIdAsync(payment.Enrollment.ProgramId);

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
            SemesterId = payment.Enrollment.SemesterId,
            ProgramId = payment.Enrollment.ProgramId,
            StudentName = user.Fullname,
            Email = user.Email,
            EnrollmentDate = payment.Enrollment.EnrollmentDate,
            ProgramName = payment.Enrollment.Program.Name,
            SemesterName = payment.Enrollment.Semester.Name,
            PaymentMethod = payment.PaymentMethod,
            Courses = courses
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
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
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            Console.WriteLine("Model in ProcessAdjustment: " + json);
            return View("Adjustment", model);
        }

        var payment = await _paymentService.GetPaymentByIdAsync(model.PaymentId);

        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        var courses = await _courseService.GetCoursesByIdsAsync(model.SelectedCourses);

        var amountPaid = courses.Sum(c => c.FeePerCredit * c.Credits);

        if (
            model.EnrollmentDate != payment.Enrollment.EnrollmentDate
            || model.ProgramName != payment.Enrollment.Program.Name
            || model.SemesterName != payment.Enrollment.Semester.Name
        )
        {
            var (success, message) = await _enrollmentService.UpdateEnrollmentAsync(
                payment.EnrollmentId,
                model.ProgramId,
                model.SemesterId,
                model.EnrollmentDate,
                model.SelectedCourses
            );
            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }
        }

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
