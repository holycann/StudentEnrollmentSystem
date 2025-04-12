using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Services;

public interface IPaymentService
{
    Task<List<Payment>> GetAllPaymentsAsync();
    Task<Payment> GetPaymentByIdAsync(int id);
    Task<List<Payment>> GetPaymentsByStudentIdAsync(string studentId);
    Task<(Payment payment, string message)> CreatePaymentAsync(MakePaymentViewModel model);
    Task<(bool success, string message)> UpdatePaymentAsync(Payment payment);
    Task<(Payment payment, string message)> ProcessPaymentAsync(MakePaymentViewModel model);
    Task<(bool success, string message)> ProcessAdjustmentAsync(
        int paymentId,
        decimal amountPaid,
        PaymentMethod? paymentMethod
    );
}

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task<Payment> GetPaymentByIdAsync(int id)
    {
        return await _context
            .Payments.Include(p => p.Enrollment)
            .ThenInclude(e => e.Program)
            .Include(p => p.Enrollment)
            .ThenInclude(e => e.Semester)
            .Include(p => p.Enrollment)
            .ThenInclude(e => e.EnrollmentCourses)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Payment>> GetPaymentsByStudentIdAsync(string studentId)
    {
        return await _context.Payments.Where(p => p.StudentId == studentId).ToListAsync();
    }

    public async Task<(Payment payment, string message)> CreatePaymentAsync(
        MakePaymentViewModel model
    )
    {
        try
        {
            var payment = new Payment
            {
                StudentId = model.StudentId,
                EnrollmentId = model.Enrollment.Id,
                Amount = model.Amount,
                Status = PaymentStatus.Pending,
            };

            if (model.PaymentMethod == PaymentMethod.Cards.ToString())
            {
                payment.PaymentMethod = PaymentMethod.Cards;
            }
            else if (model.PaymentMethod == PaymentMethod.BankTransfer.ToString())
            {
                payment.PaymentMethod = PaymentMethod.BankTransfer;
            }

            var json = JsonConvert.SerializeObject(payment, Formatting.Indented);
            Console.WriteLine("Payment Create: " + json);

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return (payment, "Payment created successfully");
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }

    public async Task<(bool success, string message)> UpdatePaymentAsync(Payment payment)
    {
        var existingPayment = await _context.Payments.FindAsync(payment.Id);
        if (existingPayment == null)
        {
            return (false, "Payment not found");
        }

        existingPayment.StudentId = payment.StudentId;
        existingPayment.Status = payment.Status;
        existingPayment.PaymentDate = payment.PaymentDate;
        existingPayment.PaymentMethod = payment.PaymentMethod;
        existingPayment.Amount = payment.Amount;
        existingPayment.EnrollmentId = payment.EnrollmentId;

        await _context.SaveChangesAsync();

        return (true, "Payment updated successfully");
    }

    public async Task<(Payment payment, string message)> ProcessPaymentAsync(
        MakePaymentViewModel model
    )
    {
        if (model.PaymentMethod == null)
        {
            return (null, "Payment method is required");
        }

        if (model.PaymentMethod == PaymentMethod.Cards.ToString())
        {
            if (
                model.CardNumber == null
                || model.CVV == null
                || model.ExpiryDate == null
                || model.CardholderName == null
            )
            {
                return (null, "Card number, CVV, expiry date, and cardholder name are required");
            }

            if (
                model.CardNumber.Replace(" ", "").Length != 16
                || model.CVV.Length > 4
                || model.CVV.Length < 3
                || model.ExpiryDate.Length != 5
                || model.CardholderName.Length < 3
            )
            {
                return (null, "Invalid card number, CVV, expiry date, or cardholder name");
            }

            var (payment, message) = await CreatePaymentAsync(model);
            if (payment == null)
            {
                return (null, message);
            }
            return (payment, message);
        }
        else if (model.PaymentMethod == PaymentMethod.BankTransfer.ToString())
        {
            if (
                model.Student == null
                || model.Student.BankAccountNumber == null
                || model.Student.BankName == null
            )
            {
                return (null, "Bank name, account number, and account name are required");
            }

            var (payment, message) = await CreatePaymentAsync(model);
            if (payment == null)
            {
                return (null, message);
            }
            return (payment, message);
        }
        else
        {
            return (null, "Invalid payment method");
        }
    }

    public async Task<(bool success, string message)> ProcessAdjustmentAsync(
        int paymentId,
        decimal amountPaid,
        PaymentMethod? paymentMethod
    )
    {
        try
        {
            var payment = await GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return (false, "Payment not found");
            }

            payment.Amount = amountPaid;
            payment.PaymentMethod = paymentMethod ?? payment.PaymentMethod;

            await _context.SaveChangesAsync();

            return (true, "Adjustment processed successfully");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
