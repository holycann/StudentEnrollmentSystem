using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
namespace StudentEnrollmentSystem.Services
{
    public interface ITeacherService
    {
        Task<Teacher> RegisterTeacherAsync(Teacher teacher);
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<Teacher> GetTeacherByIdAsync(string teacherId);
        Task<Teacher> UpdateTeacherAsync(Teacher teacher);
    }

    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher> RegisterTeacherAsync(Teacher teacher)
        {
            if (teacher == null)
            {
                throw new ArgumentNullException(nameof(teacher));
            }

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task<Teacher> GetTeacherByIdAsync(string teacherId)
        {
            return await _context.Teachers.FindAsync(teacherId);
        }


        public async Task<Teacher> UpdateTeacherAsync(Teacher teacher)
        {
            if (teacher == null)
            {
                throw new ArgumentNullException(nameof(teacher));
            }

            var existingTeacher = await _context.Teachers.FindAsync(teacher.TeacherId);
            if (existingTeacher == null)
            {
                throw new InvalidOperationException("Teacher not found.");
            }

            // Update informasi mahasiswa
            existingTeacher.Courses = teacher.Courses;
            return existingTeacher;
        }
    }
}
