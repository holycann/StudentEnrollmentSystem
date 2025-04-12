using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<List<Course>> GetCoursesByStudentIdAsync(string studentId);
        Task<List<Course>> GetCoursesByProgramIdAsync(int programId);
        Task<List<Course>> GetCoursesByIdsAsync(List<int> courseIds);
        // Task<List<Course>> GetCoursesByDepartmentIdAsync(int departmentId);
    }

    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }

        public async Task<List<Course>> GetCoursesByStudentIdAsync(string studentId)
        {
            return await _context
                .Courses.Where(c =>
                    c.EnrollmentCourses.Any(ec => ec.Enrollment.StudentId == studentId)
                )
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByProgramIdAsync(int programId)
        {
            return await _context.Courses.Where(c => c.ProgramId == programId).ToListAsync();
        }

        // public async Task<List<Course>> GetCoursesByDepartmentIdAsync(int departmentId)
        // {
        //     return await _context.Courses.Where(c => c.DepartmentId == departmentId).ToListAsync();
        // }

        public async Task<List<Course>> GetCoursesByIdsAsync(List<int> courseIds)
        {
            return await _context.Courses.Where(c => courseIds.Contains(c.Id)).ToListAsync();
        }
    }
}
