using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class Student
    {
        [Key]
        [StringLength(20)]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!; // Pastikan tidak boleh null

        [Required]
        public int ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        public virtual ProgramStudy ProgramStudy { get; set; } = null!;

        [Required]
        public int SemesterId { get; set; }
        
        [ForeignKey(nameof(SemesterId))]
        public virtual Semester Semester { get; set; } = null!;

        // Bank details (Opsional)
        [StringLength(25)] // Maksimum panjang nomor rekening
        public string? BankAccountNumber { get; set; }

        [StringLength(50)] // Maksimum panjang nama bank
        public string? BankName { get; set; }

        // Relasi dengan Enrollment
        public virtual ICollection<Enrollment> Enrollments { get; set; } =
            new HashSet<Enrollment>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<Enquiry> Enquiries { get; set; } = new HashSet<Enquiry>();

    }
}
