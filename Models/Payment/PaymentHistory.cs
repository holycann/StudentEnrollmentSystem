using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class PaymentHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PaymentId { get; set; } // Foreign Key ke Payment

        [ForeignKey(nameof(PaymentId))]
        public virtual Payment Payment { get; set; } = null!;

        [Required]
        public DateTime StatusUpdatedAt { get; set; } = DateTime.UtcNow; // Waktu perubahan status

        [Required]
        public Enums.PaymentStatus OldStatus { get; set; } // Status sebelumnya

        [Required]
        public Enums.PaymentStatus NewStatus { get; set; } // Status baru

        [StringLength(255)]
        public string Notes { get; set; } = string.Empty; // Catatan perubahan status
    }
}
