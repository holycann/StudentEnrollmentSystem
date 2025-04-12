using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IStudentService _studentService;
    private readonly IWebHostEnvironment _environment;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IWebHostEnvironment environment,
        IStudentService studentService
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
        _studentService = studentService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [Authorize]
    public IActionResult UpdateProfile()
    {
        var model = new UpdateProfileViewModels
        {
            FullName = "Sagynysh Sovet", // ������
            Email = "sagynysh@example.com",
            PhoneNumber = "87001234567"
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public IActionResult UpdateProfile(UpdateProfileViewModels model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        // ����� �������� ���������� ������ � ���� ������

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> UpdateBankDetails()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student == null)
        {
            return NotFound("Student not found");
        }

        var model = new UpdateBankDetailsViewModels
        {
            BankName = student.BankName,
            AccountNumber = student.BankAccountNumber,
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateBankDetails(UpdateBankDetailsViewModels model)
    {
        if (!ModelState.IsValid)
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            Console.WriteLine("Model in UpdateBankDetails: " + json);
            return View();
        }

        var userId = HttpContext.Session.GetString("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var student = await _studentService.GetStudentByUserIdAsync(userId);

        if (student == null)
        {
            return NotFound("Student not found");
        }

        student.BankName = model.BankName;
        student.BankAccountNumber = model.AccountNumber;

        var updatedStudent = await _studentService.UpdateStudentAsync(student);

        if (updatedStudent == null)
        {
            return NotFound("Student not found");
        }

        TempData["SuccessMessage"] = "Bank details updated successfully!";

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModels model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        return RedirectToAction("Index", "Home");
    }
}
