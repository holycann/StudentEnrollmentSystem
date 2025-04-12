using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services
{
    public interface IAuthService
    {
        Task<User> LoginAsync(string email, string password, bool? rememberMe);
        Task LogoutAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> LoginAsync(string email, string password, bool? rememberMe)
        {
            Console.WriteLine(
                $"LoginAsync: Email = {email}, Password = {password}, RememberMe = {rememberMe}"
            );

            Console.WriteLine($"Email yang dicari: {email}");

            var user = await _userManager
                .Users.Where(u => u.Email.ToLower().Trim() == email.ToLower().Trim())
                .FirstOrDefaultAsync();

            if (user == null)
            {
                Console.WriteLine("User tidak ditemukan.");
            }
            else
            {
                Console.WriteLine($"User ditemukan: {user.Email}");
            }


            if (
                user == null
                || !await _userManager
                    .Users.Where(u => u.Password.ToLower() == password.ToLower())
                    .AnyAsync()
            )
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            user.UserName = user.Email;

            await _signInManager.SignInAsync(user, isPersistent: rememberMe ?? false);

            return user;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
