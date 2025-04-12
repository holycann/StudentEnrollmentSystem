using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validasi model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("Fullname", user.Fullname);
            HttpContext.Session.SetString("Email", user.Email);

            Console.WriteLine($"User ID: {user.Id}");

            if (
                !string.IsNullOrEmpty(model.ReturnUrl)
                && Uri.IsWellFormedUriString(model.ReturnUrl, UriKind.Absolute)
            )
            {
                return Redirect(model.ReturnUrl);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth"); // atau "Index", "Home"
        }
    }
}
