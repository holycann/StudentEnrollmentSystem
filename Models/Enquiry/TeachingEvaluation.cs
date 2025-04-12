using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Models;

public class TeachingEvaluation
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int TeachingEffectiveness { get; set; }
    
    [Range(1, 5)]
    public int CourseContent { get; set; }
    
    [Range(1, 5)]
    public int LearningMaterials { get; set; }
    
    [Range(1, 5)]
    public int CommunicationSkills { get; set; }
    
    [Range(1, 5)]
    public int Engagement { get; set; }
    
    [Range(1, 5)]
    public int Feedback { get; set; }
    
    [Range(1, 5)]
    public int TimeManagement { get; set; }
    
    [Range(1, 5)]
    public int TechnologyUse { get; set; }
    
    [StringLength(1000)]
    public string Strengths { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string AreasForImprovement { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string AdditionalComments { get; set; } = string.Empty;
    
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Student Student { get; set; }
    public virtual Course Course { get; set; }
}
