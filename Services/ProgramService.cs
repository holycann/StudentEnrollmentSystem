using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services
{
    public interface IProgramService
    {
        Task<ProgramStudy> AddProgramAsync(ProgramStudy program);
        Task<ProgramStudy> UpdateProgramAsync(ProgramStudy program);
        Task<bool> DeleteProgramAsync(int id);
        Task<ProgramStudy> GetProgramByIdAsync(int id);
        Task<List<ProgramStudy>> GetAllProgramsAsync();
    }

    public class ProgramService : IProgramService
    {
        private readonly ApplicationDbContext _context;

        public ProgramService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProgramStudy> AddProgramAsync(ProgramStudy program)
        {
            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            _context.ProgramStudies.Add(program);
            await _context.SaveChangesAsync();
            return program;
        }

        public async Task<ProgramStudy> UpdateProgramAsync(ProgramStudy program)
        {
            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            var existingProgram = await _context.ProgramStudies.FindAsync(program.Id);
            if (existingProgram == null)
            {
                throw new KeyNotFoundException("Program not found.");
            }

            existingProgram.Name = program.Name;
            existingProgram.Description = program.Description;
            existingProgram.DurationInYears = program.DurationInYears;

            await _context.SaveChangesAsync();
            return existingProgram;
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            var program = await _context.ProgramStudies.FindAsync(id);
            if (program == null)
            {
                return false; // Program not found
            }

            _context.ProgramStudies.Remove(program);
            await _context.SaveChangesAsync();
            return true; // Program successfully deleted
        }

        public async Task<ProgramStudy> GetProgramByIdAsync(int id)
        {
            return await _context.ProgramStudies.FindAsync(id);
        }

        public async Task<List<ProgramStudy>> GetAllProgramsAsync()
        {
            return await _context.ProgramStudies.ToListAsync();
        }
    }
}
