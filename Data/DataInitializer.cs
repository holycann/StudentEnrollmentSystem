using System;
using System.Collections.Generic;
using Entityonlineform.Models;
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
                var rand = new Random();
                var courses = new List<Course>
                {
                   // Computer Science Courses
                new Course {CourseName = "Introduction to Computer Science", CourseCode = "CS101", Credits = 3, Capacity = 30, EnrolledCount = 0, Description = "An introductory course to computer science principles", SemesterId = 1 },
                new Course {CourseName = "Data Structures and Algorithms", CourseCode = "CS201", Credits = 4, Capacity = 25, EnrolledCount = 0, Description = "Study of data structures and algorithms", SemesterId = 1 },
                new Course {CourseName = "Database Systems", CourseCode = "CS301", Credits = 3, Capacity = 20, EnrolledCount = 0, Description = "Introduction to database design and implementation", SemesterId = 1 },
                new Course {CourseName = "Web Development", CourseCode = "CS310", Credits = 3, Capacity = 25, EnrolledCount = 0, Description = "Introduction to web development technologies and frameworks", SemesterId = 2 },
                new Course {CourseName = "Operating Systems", CourseCode = "CS320", Credits = 4, Capacity = 20, EnrolledCount = 0, Description = "Study of operating system principles and design", SemesterId = 2 },
                new Course {CourseName = "Computer Networks", CourseCode = "CS330", Credits = 3, Capacity = 22, EnrolledCount = 0, Description = "Introduction to computer networking concepts and protocols", SemesterId = 2 },
                new Course {CourseName = "Artificial Intelligence", CourseCode = "CS401", Credits = 4, Capacity = 18, EnrolledCount = 0, Description = "Introduction to artificial intelligence concepts and algorithms", SemesterId = 3 },
                new Course {CourseName = "Machine Learning", CourseCode = "CS410", Credits = 4, Capacity = 15, EnrolledCount = 0, Description = "Study of machine learning algorithms and applications", SemesterId = 3 },

                // Mathematics Courses
                new Course {CourseName = "Calculus I", CourseCode = "MATH101", Credits = 4, Capacity = 35, EnrolledCount = 0, Description = "Introduction to differential calculus", SemesterId = 1 },
                new Course { CourseName = "Calculus II", CourseCode = "MATH102", Credits = 4, Capacity = 30, EnrolledCount = 0, Description = "Introduction to integral calculus", SemesterId = 1 },
                new Course { CourseName = "Linear Algebra", CourseCode = "MATH201", Credits = 3, Capacity = 25, EnrolledCount = 0, Description = "Study of vector spaces and linear transformations", SemesterId = 1 },
                new Course { CourseName = "Discrete Mathematics", CourseCode = "MATH210", Credits = 3, Capacity = 28, EnrolledCount = 0, Description = "Mathematical structures that are fundamentally discrete", SemesterId = 1 },

                // Business Courses
                new Course { CourseName = "Introduction to Business", CourseCode = "BUS101", Credits = 3, Capacity = 40, EnrolledCount = 0, Description = "Overview of business concepts and practices", SemesterId = 1 },
                new Course { CourseName = "Principles of Marketing", CourseCode = "BUS201", Credits = 3, Capacity = 35, EnrolledCount = 0, Description = "Introduction to marketing concepts and strategies", SemesterId = 1 },
                new Course { CourseName = "Financial Accounting", CourseCode = "BUS210", Credits = 3, Capacity = 30, EnrolledCount = 0, Description = "Introduction to financial accounting principles", SemesterId = 1 },

                // Science Courses
                new Course { CourseName = "General Physics I", CourseCode = "PHYS101", Credits = 4, Capacity = 30, EnrolledCount = 0, Description = "Introduction to mechanics and thermodynamics", SemesterId = 1 },
                new Course { CourseName = "General Chemistry I", CourseCode = "CHEM101", Credits = 4, Capacity = 28, EnrolledCount = 0, Description = "Introduction to chemical principles and reactions", SemesterId = 1 },
                new Course { CourseName = "Introduction to Biology", CourseCode = "BIO101", Credits = 4, Capacity = 32, EnrolledCount = 0, Description = "Introduction to biological concepts and principles", SemesterId = 1 },

                // Humanities Courses
                new Course { CourseName = "Introduction to Philosophy", CourseCode = "PHIL101", Credits = 3, Capacity = 35, EnrolledCount = 0, Description = "Introduction to philosophical concepts and thinking", SemesterId = 1 },
                new Course { CourseName = "World History", CourseCode = "HIST101", Credits = 3, Capacity = 40, EnrolledCount = 0, Description = "Survey of major historical events and developments", SemesterId = 1 }
                };

                foreach (var course in courses)
                {
                    course.Fee = rand.Next(500, 2001);
                    course.ProgramStudyId = programStudiesList[rand.Next(programStudiesList.Count)].Id;
                }

                context.Courses.AddRange(courses);
                context.SaveChanges();
            }
            var coursesList = context.Courses.ToList();

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

            if (!context.AddDropRecords.Any())
            {
                var addDropRecords = new List<AddDropRecord>
                {
                    new AddDropRecord
                    {
                        StudentId = "S002",
                        CourseId = coursesList[0].CourseId,
                        Action = "Add",
                        ActionDate = DateTime.Now,
                        Reason = "Enrolled in the course",
                    },
                    new AddDropRecord
                    {
                        StudentId = "S002",
                        CourseId = coursesList[1].CourseId,
                        Action = "Add",
                        ActionDate = DateTime.Now,
                        Reason = "Enrolled in the course",
                    },
                    new AddDropRecord
                    {
                        StudentId = "S001",
                        CourseId = coursesList[2].CourseId,
                        Action = "Drop",
                        ActionDate = DateTime.Now,
                        Reason = "Dropped the course",
                    },
                    new AddDropRecord
                    {
                        StudentId = "S001",
                        CourseId = coursesList[3].CourseId,
                        Action = "Add",
                        ActionDate = DateTime.Now,
                        Reason = "Enrolled in the course",
                    },
                    new AddDropRecord
                    {
                        StudentId = "S001",
                        CourseId = coursesList[4].CourseId,
                        Action = "Add",
                        ActionDate = DateTime.Now,
                        Reason = "Enrolled in the course",
                    },
                    new AddDropRecord
                    {
                        StudentId = "S001",
                        CourseId = coursesList[5].CourseId,
                        Action = "Drop",
                        ActionDate = DateTime.Now,
                        Reason = "Dropped the course",
                    },
                };
                context.AddDropRecords.AddRange(addDropRecords);
                context.SaveChanges();
            }

            if (!context.Enrollments.Any())
            {
                var enrollments = new List<Enrollment>
                {
                    new Enrollment
                    {
                        StudentId = "S002",
                        CourseId = coursesList[0].CourseId,
                        EnrollmentDate = DateTime.Now,
                        Status = "Enrolled",
                    },
                    new Enrollment
                    {
                        StudentId = "S002",
                        CourseId = coursesList[1].CourseId,
                        EnrollmentDate = DateTime.Now,
                        Status = "Enrolled",
                    },
                    new Enrollment
                    {
                        StudentId = "S001",
                        CourseId = coursesList[2].CourseId,
                        EnrollmentDate = DateTime.Now,
                        Status = "Dropped",
                    },
                    new Enrollment
                    {
                        StudentId = "S001",
                        CourseId = coursesList[3].CourseId,
                        EnrollmentDate = DateTime.Now,
                        Status = "Enrolled",
                    },
                    new Enrollment
                    {
                        StudentId = "S001",
                        CourseId = coursesList[4].CourseId,
                        EnrollmentDate = DateTime.Now,
                        Status = "Enrolled",
                    },
                    new Enrollment
                    {
                        StudentId = "S001",
                        CourseId = coursesList[5].CourseId,
                        EnrollmentDate = DateTime.Now,
                        Status = "Dropped",
                    },
                };
                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
            }

            if (!context.Payments.Any())
            {
                var payments = new List<Payment>
                {
                    new Payment
                    {
                        StudentId = "S002",
                        SemesterId = semestersList[0].Id,
                        Amount = 1000,
                        PaymentMethod = PaymentMethod.Cards,
                        Status = PaymentStatus.Pending,
                    }
                };
                context.Payments.AddRange(payments);
                context.SaveChanges();
            }
        }
    }
}
