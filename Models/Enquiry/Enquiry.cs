using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class Enquiry
{
    public int Id { get; set; } = 0;
    public string StudentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
    public Enums.EnquiryStatus Status { get; set; } = Enums.EnquiryStatus.Pending;
    public string Response { get; set; } = string.Empty;

    // Navigation properties
    [JsonIgnore]
    public virtual Student Student { get; set; } = null!;
}