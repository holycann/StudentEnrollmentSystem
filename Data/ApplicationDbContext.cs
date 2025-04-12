using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // User-related Tables
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    // Study Programs
    public DbSet<ProgramStudy> ProgramStudies { get; set; }
    public DbSet<Semester> Semesters { get; set; }

    // Course
    public DbSet<Course> Courses { get; set; }
    public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
    public DbSet<CourseSchedule> CourseSchedules { get; set; }

    // Enrollment
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<EnrollmentCourse> EnrollmentCourses { get; set; }

    // Payment
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentHistory> PaymentHistories { get; set; }

    // Enquiry
    public DbSet<Enquiry> Enquiries { get; set; }
    public DbSet<TeachingEvaluation> TeachingEvaluations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Student>()
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relasi Student ke Semester
        modelBuilder
            .Entity<Student>()
            .HasOne(s => s.Semester)
            .WithMany(s => s.Students)
            .HasForeignKey(s => s.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Teacher>()
            .HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<Teacher>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relasi Student ke ProgramStudy
        modelBuilder
            .Entity<Student>()
            .HasOne(s => s.ProgramStudy)
            .WithMany(p => p.Students)
            .HasForeignKey(s => s.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many Relationship Course - Prerequisites
        modelBuilder
            .Entity<CoursePrerequisite>()
            .HasKey(cp => new { cp.CourseId, cp.PrerequisiteCourseId });

        modelBuilder
            .Entity<CoursePrerequisite>()
            .HasOne(cp => cp.Course)
            .WithMany(c => c.PrerequisiteCourses)
            .HasForeignKey(cp => cp.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<CoursePrerequisite>()
            .HasOne(cp => cp.PrerequisiteCourses)
            .WithMany()
            .HasForeignKey(cp => cp.PrerequisiteCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many Relationship Enrollment - Course
        modelBuilder.Entity<EnrollmentCourse>().HasKey(ec => new { ec.EnrollmentId, ec.CourseId });

        modelBuilder
            .Entity<EnrollmentCourse>()
            .HasOne(ec => ec.Enrollment)
            .WithMany(e => e.EnrollmentCourses)
            .HasForeignKey(ec => ec.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<EnrollmentCourse>()
            .HasOne(ec => ec.Course)
            .WithMany(c => c.EnrollmentCourses)
            .HasForeignKey(ec => ec.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Enrollment ke Student
        modelBuilder
            .Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relasi Payment ke Student
        modelBuilder
            .Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Payment ke Enrollment
        modelBuilder
            .Entity<Payment>()
            .HasOne(p => p.Enrollment)
            .WithMany(e => e.Payments)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Enquiry ke Student
        modelBuilder
            .Entity<Enquiry>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enquiries)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Teaching Evaluation ke Student & Course
        modelBuilder
            .Entity<TeachingEvaluation>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<TeachingEvaluation>()
            .HasOne(e => e.Course)
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Course Schedule ke Course
        modelBuilder
            .Entity<CourseSchedule>()
            .HasOne(cs => cs.Course)
            .WithMany(c => c.CourseSchedules)
            .HasForeignKey(cs => cs.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
