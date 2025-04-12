using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Services
{
    public interface IStatementService
    {
        // Task<StatementViewModel> GenerateStatementAsync(string studentId);
    }

    public class StatementService : IStatementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IProgramService _programService;

        public StatementService(
            ApplicationDbContext context,
            IEnrollmentService enrollmentService,
            IProgramService programService
        )
        {
            _context = context;
            _enrollmentService = enrollmentService;
            _programService = programService;
        }

        //     public async Task<StatementViewModel> GenerateStatementAsync(string studentId)
        //     {
        //         // Ambil data mahasiswa
        //         var student = await _context
        //             .Users.Include(u => u.Enrollments)
        //             .ThenInclude(e => e.Course)
        //             .FirstOrDefaultAsync(u => u.Id == studentId);

        //         if (student == null)
        //         {
        //             throw new KeyNotFoundException("Student not found.");
        //         }

        //         // Ambil program studi mahasiswa
        //         var programStudy = await _programService.GetProgramByIdAsync(student.ProgramStudyId);

        //         // Hitung total biaya dan total yang dibayar
        //         var enrollments = student
        //             .Enrollments.Select(e => new EnrollmentStatement
        //             {
        //                 EnrollmentId = e.Id,
        //                 CourseCode = e.Course.Code,
        //                 CourseName = e.Course.Name,
        //                 Credits = e.Course.Credits,
        //                 Fee = e.Course.Fee,
        //                 EnrollmentDate = e.EnrollmentDate,
        //                 Status = e.Status,
        //                 Schedule = e.Schedule,
        //             })
        //             .ToList();

        //         var totalFees = enrollments.Sum(e => e.Fee);
        //         var totalPaid = await _context
        //             .Payments.Where(p => p.StudentId == studentId)
        //             .SumAsync(p => p.Amount);

        //         // Buat StatementViewModel
        //         var statement = new StatementViewModel
        //         {
        //             Student = new StudentViewModel
        //             {
        //                 Id = student.Id,
        //                 Name = student.Name,
        //                 Email = student.Email,
        //             },
        //             ProgramStudy = new ProgramStudyViewModel
        //             {
        //                 Id = programStudy.Id,
        //                 Name = programStudy.Name,
        //                 Description = programStudy.Description,
        //                 DurationInYears = programStudy.DurationInYears,
        //             },
        //             GeneratedDate = DateTime.Now,
        //             TotalFees = totalFees,
        //             TotalPaid = totalPaid,
        //             Enrollments = enrollments,
        //             Registration = new RegistrationSummary
        //             {
        //                 TotalCredits = enrollments.Sum(e => e.Credits),
        //                 // TotalCourses = enrollments.Count,
        //                 RegistrationDate = student.RegistrationDate,
        //                 AcademicTerm = student.AcademicTerm,
        //                 EnrolledCourses = enrollments.Select(e => e.CourseName).ToList(),
        //                 WeeklySchedule = student.WeeklySchedule,
        //             },
        //         };

        //         return statement;
        //     }
        // }
    }
}
