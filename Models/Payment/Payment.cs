using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty; // Foreign Key ke Student

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [Required]
        public int EnrollmentId { get; set; } // Foreign Key ke Enrollment

        [ForeignKey(nameof(EnrollmentId))]
        public virtual Enrollment Enrollment { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Jumlah pembayaran harus lebih dari 0.")]
        public decimal Amount { get; set; }

        [Required]
        public Enums.PaymentStatus Status { get; set; }

        [Required]
        public Enums.PaymentMethod PaymentMethod { get; set; } // Pakai Enum, bukan string!

        public DateTime? PaymentDate { get; set; } // nullable
    }
}
