
using System.ComponentModel.DataAnnotations;
using Entityonlineform.Models;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class AddDropRecordViewModel
{
    public IList<AddDropRecord> AddDropRecords { get; set; } = default!;
    public Student CurrentStudent { get; set; } = default!;
}

