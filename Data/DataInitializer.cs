using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Data
{
    public static class DataInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (!context.ProgramStudies.Any())
            {
                var programStudies = new List<ProgramStudy>
                {
                    new ProgramStudy
                    {
                        Name = "Computer Science",
                        Description = "Study of computer systems and programming.",
                        DurationInYears = 3,
                    },
                    new ProgramStudy
                    {
                        Name = "Information Technology",
                        Description = "Focus on IT systems and management.",
                        DurationInYears = 3,
                    },
                    new ProgramStudy
                    {
                        Name = "Software Engineering",
                        Description = "Focus on software development and engineering principles.",
                        DurationInYears = 4,
                    },
                    new ProgramStudy
                    {
                        Name = "Data Science",
                        Description = "Study of data analysis and machine learning.",
                        DurationInYears = 3,
                    },
                };
                context.ProgramStudies.AddRange(programStudies);
                context.SaveChanges();
            }
            var programStudiesList = context.ProgramStudies.ToList();

            if (!context.Semesters.Any())
            {
                var semesters = new List<Semester>
                {
                    new Semester
                    {
                        Name = "Fall 2023",
                        SemesterNumber = 1,
                        StartDate = new DateTime(2023, 9, 1),
                        EndDate = new DateTime(2023, 12, 15),
                    },
                    new Semester
                    {
                        Name = "Spring 2024",
                        SemesterNumber = 2,
                        StartDate = new DateTime(2024, 1, 15),
                        EndDate = new DateTime(2024, 5, 15),
                    },
                    new Semester
                    {
                        Name = "Fall 2024",
                        SemesterNumber = 3,
                        StartDate = new DateTime(2024, 1, 15),
                        EndDate = new DateTime(2024, 5, 15),
                    },
                    new Semester
                    {
                        Name = "Fall 2025",
                        SemesterNumber = 4,
                        StartDate = new DateTime(2025, 1, 15),
                        EndDate = new DateTime(2025, 5, 15),
                    },
                    new Semester
                    {
                        Name = "Spring 2025",
                        SemesterNumber = 5,
                        StartDate = new DateTime(2025, 1, 15),
                        EndDate = new DateTime(2025, 5, 15),
                    },
                };

                context.Semesters.AddRange(semesters);
                context.SaveChanges();
            }
            var semestersList = context.Semesters.ToList();

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Email = "student1@example.com",
                        Password = "password1",
                        Fullname = "Student 1",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        City = "New York",
                        Country = "USA",
                        Address = "123 Main St",
                        PostalCode = "10001",
                    },
                    new User
                    {
                        Email = "student2@example.com",
                        Password = "password2",
                        Fullname = "Student 2",
                        Gender = "Female",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        City = "New York",
                        Country = "USA",
                        Address = "123 Main St",
                        PostalCode = "10001",
                    },
                    new User
                    {
                        Email = "teacher1@example.com",
                        Password = "password3",
                        Fullname = "Teacher 1",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        City = "New York",
                        Country = "USA",
                        Address = "123 Main St",
                        PostalCode = "10001",
                    },
                    new User
                    {
                        Email = "teacher2@example.com",
                        Password = "password4",
                        Fullname = "Teacher 2",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        City = "New York",
                        Country = "USA",
                        Address = "123 Main St",
                        PostalCode = "10001",
                    },
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
            var usersList = context.Users.ToList();

            if (!context.Teachers.Any())
            {
                var teachers = new List<Teacher>
                {
                    new Teacher
                    {
                        TeacherId = "T001",
                        UserId = usersList.Where(u => u.Email == "teacher1@example.com").FirstOrDefault().Id,
                        Department = "Computer Science",
                    },
                    new Teacher
                    {
                        TeacherId = "T002",
                        UserId = usersList.Where(u => u.Email == "teacher2@example.com").FirstOrDefault().Id,
                        Department = "Information Technology",
                    },
                };
                context.Teachers.AddRange(teachers);
                context.SaveChanges();
            }
            var teachersList = context.Teachers.ToList();

            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    new Course
                    {
                        CourseCode = "CS101",
                        Name = "Introduction to Computer Science",
                        Description = "Basic concepts of computer science.",
                        Credits = 3,
                        FeePerCredit = 50,
                        ProgramId = programStudiesList[0].Id,
                        TeacherId = teachersList[0].TeacherId,
                    },
                    new Course
                    {
                        CourseCode = "IT201",
                        Name = "Information Systems",
                        Description = "Study of information systems.",
                        Credits = 3,
                        FeePerCredit = 100,
                        ProgramId = programStudiesList[1].Id,
                        TeacherId = teachersList[1].TeacherId,
                    },
                    new Course
                    {
                        CourseCode = "SE301",
                        Name = "Software Development",
                        Description = "Principles of software development.",
                        Credits = 3,
                        FeePerCredit = 50,
                        ProgramId = programStudiesList[2].Id,
                        TeacherId = teachersList[1].TeacherId,
                    },
                    new Course
                    {
                        CourseCode = "DS401",
                        Name = "Data Analysis",
                        Description = "Techniques for data analysis.",
                        Credits = 3,
                        FeePerCredit = 100,
                        ProgramId = programStudiesList[3].Id,
                        TeacherId = teachersList[0].TeacherId,
                    },
                };
                context.Courses.AddRange(courses);
                context.SaveChanges();
            }
            var coursesList = context.Courses.ToList();

            if (!context.CoursePrerequisites.Any())
            {
                var prerequisites = new List<CoursePrerequisite>
                {
                    new CoursePrerequisite
                    {
                        CourseId = coursesList[1].Id,
                        PrerequisiteCourseId = coursesList[0].Id,
                        SemesterId = semestersList[0].Id,
                    }, // IT201 membutuhkan CS101
                    new CoursePrerequisite
                    {
                        CourseId = coursesList[2].Id,
                        PrerequisiteCourseId = coursesList[1].Id,
                        SemesterId = semestersList[0].Id,
                    }, // SE301 membutuhkan IT201
                };
                context.CoursePrerequisites.AddRange(prerequisites);
                context.SaveChanges();
            }

            if (!context.CourseSchedules.Any())
            {
                var schedules = new List<CourseSchedule>
                {
                    new CourseSchedule
                    {
                        CourseId = coursesList[0].Id,
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = TimeSpan.FromHours(9),
                        EndTime = TimeSpan.FromHours(11),
                        Room = "Room 1",
                    },
                    new CourseSchedule
                    {
                        CourseId = coursesList[1].Id,
                        DayOfWeek = DayOfWeek.Tuesday,
                        StartTime = TimeSpan.FromHours(10),
                        EndTime = TimeSpan.FromHours(12),
                        Room = "Room 2",
                    },
                    new CourseSchedule
                    {
                        CourseId = coursesList[2].Id,
                        DayOfWeek = DayOfWeek.Wednesday,
                        StartTime = TimeSpan.FromHours(13),
                        EndTime = TimeSpan.FromHours(15),
                        Room = "Room 3",
                    },
                    new CourseSchedule
                    {
                        CourseId = coursesList[3].Id,
                        DayOfWeek = DayOfWeek.Thursday,
                        StartTime = TimeSpan.FromHours(14),
                        EndTime = TimeSpan.FromHours(16),
                        Room = "Room 4",
                    },
                };
                context.CourseSchedules.AddRange(schedules);
                context.SaveChanges();
            }

            if (!context.Students.Any())
            {
                var students = new List<Student>
                {
                    new Student
                    {
                        StudentId = "S001",
                        UserId = usersList.Where(u => u.Email == "student1@example.com").FirstOrDefault().Id,
                        ProgramId = programStudiesList[0].Id,
                        SemesterId = semestersList[0].Id,
                        BankAccountNumber = "1234567890",
                        BankName = "Bank of America",
                    },
                    new Student
                    {
                        StudentId = "S002",
                        UserId = usersList.Where(u => u.Email == "student2@example.com").FirstOrDefault().Id,
                        ProgramId = programStudiesList[1].Id,
                        SemesterId = semestersList[1].Id,
                        BankAccountNumber = "1234567890",
                        BankName = "Bank of America",
                    },
                };
                context.Students.AddRange(students);
                context.SaveChanges();
            }
            var studentsList = context.Students.ToList();

            if (!context.Enrollments.Any())
            {
                var enrollments = new List<Enrollment>
                {
                    new Enrollment
                    {
                        StudentId = studentsList[0].StudentId,
                        ProgramId = programStudiesList[0].Id,
                        SemesterId = semestersList[0].Id,
                        TotalCredits = coursesList[0].Credits + coursesList[1].Credits,
                        EnrollmentDate = DateTime.UtcNow,
                        Status = EnrollmentStatus.Pending,
                    },
                    new Enrollment
                    {
                        StudentId = studentsList[1].StudentId,
                        ProgramId = programStudiesList[1].Id,
                        SemesterId = semestersList[1].Id,
                        TotalCredits = coursesList[0].Credits + coursesList[1].Credits,
                        EnrollmentDate = DateTime.UtcNow,
                        Status = EnrollmentStatus.Completed,
                    },
                    new Enrollment
                    {
                        StudentId = studentsList[1].StudentId,
                        ProgramId = programStudiesList[1].Id,
                        SemesterId = semestersList[1].Id,
                        TotalCredits = coursesList[2].Credits + coursesList[3].Credits,
                        EnrollmentDate = DateTime.UtcNow,
                        Status = EnrollmentStatus.Pending,
                    },
                };
                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
            }
            var enrollmentsList = context.Enrollments.ToList();

            if (!context.EnrollmentCourses.Any())
            {
                var enrollmentCourses = new List<EnrollmentCourse>
                {
                    new EnrollmentCourse
                    {
                        EnrollmentId = enrollmentsList[0].Id,
                        CourseId = coursesList[0].Id,
                    },
                    new EnrollmentCourse
                    {
                        EnrollmentId = enrollmentsList[0].Id,
                        CourseId = coursesList[1].Id,
                    },
                    new EnrollmentCourse
                    {
                        EnrollmentId = enrollmentsList[1].Id,
                        CourseId = coursesList[0].Id,
                    },
                    new EnrollmentCourse
                    {
                        EnrollmentId = enrollmentsList[1].Id,
                        CourseId = coursesList[1].Id,
                    },
                    new EnrollmentCourse
                    {
                        EnrollmentId = enrollmentsList[2].Id,
                        CourseId = coursesList[2].Id,
                    },
                    new EnrollmentCourse
                    {
                        EnrollmentId = enrollmentsList[2].Id,
                        CourseId = coursesList[3].Id,
                    },
                };
                context.EnrollmentCourses.AddRange(enrollmentCourses);
                context.SaveChanges();
            }

            // if (!context.Payments.Any())
            // {
            //     var payments = new List<Payment>
            //     {
            //         new Payment
            //         {
            //             StudentId = studentsList[0].StudentId,
            //             EnrollmentId = enrollmentsList[0].Id,
            //             Amount = 100,
            //             PaymentDate = DateTime.UtcNow,
            //             PaymentMethod = PaymentMethod.CreditCard,
            //             Status = PaymentStatus.Completed,
            //         },
            //         new Payment
            //         {
            //             StudentId = studentsList[1].StudentId,
            //             EnrollmentId = enrollmentsList[0].Id,
            //             Amount = 100,
            //             PaymentMethod = PaymentMethod.CreditCard,
            //             PaymentDate = null,
            //             Status = PaymentStatus.Pending,
            //         },
            //     };

            //     Console.WriteLine($"Payment Date: {payments[1].PaymentDate}"); // cek isinya null atau tidak

            //     context.Payments.AddRange(payments);
            //     context.SaveChanges();
            // }

            if (!context.Enquiries.Any())
            {
                var enquiries = new List<Enquiry>
                {
                    new Enquiry
                    {
                        StudentId = studentsList[0].StudentId,
                        Name = "Student 1",
                        Email = "student1@example.com",
                        Subject = "Computer Science",
                        Message = "What are the prerequisites for CS101?",
                        SubmissionDate = DateTime.UtcNow,
                        Status = EnquiryStatus.Pending,
                        Response = "No prerequisites.",
                    },
                    new Enquiry
                    {
                        StudentId = studentsList[1].StudentId,
                        Name = "Student 2",
                        Email = "student2@example.com",
                        Subject = "Information Technology",
                        Message = "How do I enroll in courses?",
                        SubmissionDate = DateTime.UtcNow,
                        Status = EnquiryStatus.Pending,
                        Response = "You can enroll through the student portal.",
                    },
                };
                context.Enquiries.AddRange(enquiries);
                context.SaveChanges();
            }
        }
    }
}
