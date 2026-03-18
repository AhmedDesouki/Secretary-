using System.ComponentModel.DataAnnotations;

namespace Clinic_appointment.ViewModels;

public class CreateDoctorViewModel
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [DataType(DataType.Time)]
    [Display(Name = "Start")]
    public TimeOnly StartTime { get; set; } = new(9, 0);

    [DataType(DataType.Time)]
    [Display(Name = "End")]
    public TimeOnly EndTime { get; set; } = new(17, 0);

    public List<ScheduleDayInputViewModel> Days { get; set; } = ScheduleDayInputViewModel.CreateDefaultWeek();
}

public class ScheduleDayInputViewModel
{
    [Required]
    public string DayOfWeek { get; set; } = string.Empty;

    [Display(Name = "Work?")]
    public bool IsSelected { get; set; }

    public static List<ScheduleDayInputViewModel> CreateDefaultWeek() =>
    [
        new() { DayOfWeek = "Sunday" },
        new() { DayOfWeek = "Monday" },
        new() { DayOfWeek = "Tuesday" },
        new() { DayOfWeek = "Wednesday" },
        new() { DayOfWeek = "Thursday" },
        new() { DayOfWeek = "Friday" },
        new() { DayOfWeek = "Saturday" },
    ];
}

