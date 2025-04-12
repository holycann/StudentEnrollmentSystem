using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;

namespace StudentEnrollmentSystem.Services;

public interface IEnquiryService
{
}

public class EnquiryService : IEnquiryService
{
    private readonly ApplicationDbContext _context;

    public EnquiryService(ApplicationDbContext context)
    {
        _context = context;
    }
}
