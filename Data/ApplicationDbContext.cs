using Entityonlineform.Models;
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
    public DbSet<AddDropRecord> AddDropRecords { get; set; }

    // Enrollment
    public DbSet<Enrollment> Enrollments { get; set; }

    // Payment
    public DbSet<Payment> Payments { get; set; }

    // Enquiry
    public DbSet<Enquiry> Enquiries { get; set; }

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

        // Relasi Payment ke Semester
        modelBuilder
            .Entity<Payment>()
            .HasOne(p => p.Semester)
            .WithMany()
            .HasForeignKey(p => p.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relasi Course Schedule ke Course
        modelBuilder
            .Entity<AddDropRecord>()
            .HasOne(adr => adr.Course)
            .WithMany()
            .HasForeignKey(adr => adr.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
