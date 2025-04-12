using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;
using StudentEnrollmentSystem.Models.ViewModels;
using StudentEnrollmentSystem.Services;

namespace StudentEnrollmentSystem.Controllers;

[Authorize]
public class StatementsController : Controller
{
    public IActionResult ClassTimetable()
    {
        var Classes = new List<ClassSchedule>
        {
            new ClassSchedule { Day = "Monday", Time = "09:00 - 10:30", Course = "Mathematics", Lecturer = "Dr. Alim", Room = "A101" },
            new ClassSchedule { Day = "Tuesday", Time = "11:00 - 12:30", Course = "Programming", Lecturer = "Ms. Aida", Room = "B202" },
            new ClassSchedule { Day = "Wednesday", Time = "14:00 - 15:30", Course = "Database Systems", Lecturer = "Mr. Nurlan", Room = "C303" }
        };

        return View(Classes);
    }

    public IActionResult StudentStatement()
    {

        var StudentStatements = new StudentStatement
        {
            Transactions = new List<Transaction>
                {
                    new Transaction { Date = DateTime.Now.AddDays(-10), Description = "Tuition Payment", Amount = -100000 },
                    new Transaction { Date = DateTime.Now.AddDays(-5), Description = "Library Fine", Amount = -1500 },
                    new Transaction { Date = DateTime.Now.AddDays(-1), Description = "Scholarship", Amount = 120000 }
                }
        };

        decimal TotalBalance = 0;

        foreach (var t in StudentStatements.Transactions)
        {
            TotalBalance += t.Amount;
        }

        StudentStatements.TotalBalance = TotalBalance;

        return View(StudentStatements);
    }
}
