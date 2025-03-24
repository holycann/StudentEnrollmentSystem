namespace StudentEnrollmentSystem.Models;

public class Enrollment
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public int CourseId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public Enums.EnrollmentStatus Status { get; set; } = Enums.EnrollmentStatus.Pending;
    public string Notes { get; set; } = string.Empty;
    public int TotalCredits { get; set; }
    public decimal TotalFee { get; set; }
    public virtual ApplicationUser Student { get; set; }
    public virtual Course Course { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
}