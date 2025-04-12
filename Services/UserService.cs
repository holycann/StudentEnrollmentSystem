using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            // Simpan pengguna baru ke database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // Validasi pengguna
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Cek apakah pengguna ada
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Update informasi pengguna
            existingUser.Fullname = user.Fullname;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;
            existingUser.City = user.City;
            existingUser.PostalCode = user.PostalCode;
            existingUser.Country = user.Country;
            existingUser.Password = user.Password;

            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}
