using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services
{
    public interface IStudentService
    {
        Task<Student> RegisterStudentAsync(Student student);
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(string studentId);
        Task<Student> GetStudentByUserIdAsync(string userId);
        Task<Student> UpdateStudentAsync(Student student);
    }

    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Student> RegisterStudentAsync(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(string studentId)
        {
            return await _context
                .Students.Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Enrollments)
                .Include(s => s.ProgramStudy)
                .Include(s => s.Payments)
                .Include(s => s.Enquiries)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<Student> GetStudentByUserIdAsync(string userId)
        {
            return await _context
                .Students.Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Enrollments)
                .Include(s => s.ProgramStudy)
                .Include(s => s.Payments)
                .Include(s => s.Enquiries)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            var existingStudent = await _context.Students.FindAsync(student.StudentId);
            if (existingStudent == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            // Update informasi mahasiswa
            existingStudent.ProgramId = student.ProgramId;
            existingStudent.BankName = student.BankName;
            existingStudent.BankAccountNumber = student.BankAccountNumber;
            existingStudent.Enrollments = student.Enrollments;

            await _context.SaveChangesAsync();
            return existingStudent;
        }
    }
}
