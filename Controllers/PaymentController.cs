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
public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentService _paymentService;

    public PaymentController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IPaymentService paymentService)
    {
        _context = context;
        _userManager = userManager;
        _paymentService = paymentService;
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);

        var payments = await _context.Payments
            .Include(p => p.Enrollment)
            .ThenInclude(e => e.Course)
            .Where(p => p.StudentId == user.Id)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentViewModel
            {
                Id = p.Id,
                StudentId = p.StudentId,
                EnrollmentId = p.EnrollmentId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId,
                Status = (PaymentStatus)p.Status,
                PaymentMethod = p.PaymentMethod,
                Enrollment = new EnrollmentViewModel
                {
                    Id = p.Enrollment.Id,
                    Course = new CourseViewModel
                    {
                        CourseCode = p.Enrollment.Course.CourseCode,
                        Name = p.Enrollment.Course.Name
                    }
                }
            })
            .ToListAsync();

        var totalPaid = payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .Sum(p => p.Amount);

        var pendingPayments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == user.Id &&
                       (e.Status == (EnrollmentStatus)EnrollmentStatus.Pending || e.Status == EnrollmentStatus.Active) &&
                       !_context.Payments.Any(p => p.EnrollmentId == e.Id && p.Status == Models.Enums.PaymentStatus.Completed))
            .SumAsync(e => e.Course.Fee);

        var viewModel = new PaymentHistoryViewModel
        {
            Payments = payments,
            TotalPaid = totalPaid,
            PendingPayments = pendingPayments
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Process(int enrollmentId)
    {
        var user = await _userManager.GetUserAsync(User);
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.StudentId == user.Id);

        if (enrollment == null)
        {
            return NotFound();
        }

        // Check if payment already exists
        var existingPayment = await _context.Payments
            .AnyAsync(p => p.EnrollmentId == enrollmentId && p.Status == Models.Enums.PaymentStatus.Completed);

        if (existingPayment)
        {
            TempData["Error"] = "Payment has already been processed for this enrollment.";
            return RedirectToAction("History", "Enrollment");
        }

        var viewModel = new PaymentRequestViewModel
        {
            EnrollmentId = enrollment.Id,
            Amount = enrollment.Course.Fee,
            CourseCode = enrollment.Course.CourseCode,
            CourseName = enrollment.Course.Name
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(PaymentRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == model.EnrollmentId && e.StudentId == user.Id);

        if (enrollment == null)
        {
            return NotFound();
        }

        // Process payment through payment service
        var (success, transactionId) = await _paymentService.ProcessPaymentAsync(
            model.Amount,
            model.CardNumber,
            model.ExpiryDate,
            model.CVV,
            model.CardholderName);

        var payment = new Payment
        {
            StudentId = user.Id,
            EnrollmentId = model.EnrollmentId,
            Amount = model.Amount,
            PaymentDate = DateTime.UtcNow,
            TransactionId = transactionId,
            Status = success ? Models.Enums.PaymentStatus.Completed : Models.Enums.PaymentStatus.Failed,
            PaymentMethod = "Credit Card"
        };

        _context.Payments.Add(payment);

        if (success)
        {
            enrollment.Status = EnrollmentStatus.Active;
            TempData["Success"] = "Payment processed successfully.";
        }
        else
        {
            TempData["Error"] = "Payment processing failed. Please try again.";
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(History));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refund(int paymentId)
    {
        var user = await _userManager.GetUserAsync(User);
        var payment = await _context.Payments
            .Include(p => p.Enrollment)
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.StudentId == user.Id);

        if (payment == null || payment.Status != Models.Enums.PaymentStatus.Completed)
        {
            return NotFound();
        }

        // Process refund through payment service
        var success = await _paymentService.RefundPaymentAsync(payment.TransactionId);

        if (success)
        {
            payment.Status = PaymentStatus.Refunded;
            payment.Enrollment.Status = EnrollmentStatus.Dropped;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Refund processed successfully.";
        }
        else
        {
            TempData["Error"] = "Refund processing failed. Please try again.";
        }

        return RedirectToAction(nameof(History));
    }
}
