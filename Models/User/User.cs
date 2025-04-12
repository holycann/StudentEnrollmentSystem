using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StudentEnrollmentSystem.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public string? PostalCode { get; set; }

        public string? ProfilePicturePath { get; set; }

        public string Password { get; set; }
    }
}
