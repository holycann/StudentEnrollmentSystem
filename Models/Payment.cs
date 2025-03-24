using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; }

    [Required]
    public int EnrollmentId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public string TransactionId { get; set; }

    [Required]
    public Enums.PaymentStatus Status { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; }

    // Navigation properties
    public virtual ApplicationUser Student { get; set; }
    public virtual Enrollment Enrollment { get; set; }
}
