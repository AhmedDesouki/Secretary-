using System.ComponentModel.DataAnnotations;

namespace Clinic_appointment.ViewModels;

public class PatientViewModel
{
    [Required(ErrorMessage = "Patient name is required")]
    [StringLength(200)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Birth Date")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [Phone]
    [StringLength(20)]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    [Display(Name = "Email (optional)")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "Address (optional)")]
    public string? Address { get; set; }
}
