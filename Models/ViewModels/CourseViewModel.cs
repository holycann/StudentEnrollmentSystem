using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Course Code")]
    [StringLength(10)]
    public string CourseCode { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(1, 6)]
    public int Credits { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Fee { get; set; }

    [Display(Name = "Prerequisites")]
    public List<string> Prerequisites { get; set; } = new();

    public List<int> PrerequisiteCourseIds { get; set; } = new();
}

public class CourseListViewModel
{
    public IEnumerable<CourseViewModel> Courses { get; set; }
    public string SearchTerm { get; set; }
    public string SortOrder { get; set; }
    public int? CreditFilter { get; set; }
    public decimal? MaxFee { get; set; }
    public bool ShowAvailableOnly { get; set; }
}
