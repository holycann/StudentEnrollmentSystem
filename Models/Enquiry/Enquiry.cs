using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Models
{
    public class Enquiry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public EnquiryStatus Status { get; set; } = EnquiryStatus.Pending;
    }

    public enum EnquiryStatus
    {
        Pending,
        InProgress,
        Resolved,
        Closed
    }
}