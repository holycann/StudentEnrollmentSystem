using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services
{
    public interface ISemesterService
    {
        Task<Semester> AddSemesterAsync(Semester semester);
        Task<Semester> UpdateSemesterAsync(Semester semester);
        Task<bool> DeleteSemesterAsync(int id);
        Task<Semester> GetSemesterByIdAsync(int id);
        Task<List<Semester>> GetAllSemestersAsync();
    }

    public class SemesterService : ISemesterService
    {
        private readonly ApplicationDbContext _context;

        public SemesterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Semester> AddSemesterAsync(Semester semester)
        {
            if (semester == null)
            {
                throw new ArgumentNullException(nameof(semester));
            }

            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
            return semester;
        }

        public async Task<Semester> UpdateSemesterAsync(Semester semester)
        {
            if (semester == null)
            {
                throw new ArgumentNullException(nameof(semester));
            }

            var existingSemester = await _context.Semesters.FindAsync(semester.Id);
            if (existingSemester == null)
            {
                throw new KeyNotFoundException("Semester not found.");
            }

            existingSemester.Name = semester.Name;
            existingSemester.SemesterNumber = semester.SemesterNumber;
            existingSemester.StartDate = semester.StartDate;
            existingSemester.EndDate = semester.EndDate;

            await _context.SaveChangesAsync();
            return existingSemester;
        }

        public async Task<bool> DeleteSemesterAsync(int id)
        {
            var semester = await _context.Semesters.FindAsync(id);
            if (semester == null)
            {
                return false; // Semester not found
            }

            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();
            return true; // Semester successfully deleted
        }

        public async Task<Semester> GetSemesterByIdAsync(int id)
        {
            return await _context.Semesters.FindAsync(id);
        }

        public async Task<List<Semester>> GetAllSemestersAsync()
        {
            return await _context.Semesters.ToListAsync();
        }
    }
}