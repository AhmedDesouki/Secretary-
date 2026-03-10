using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_appointment.Models;

public class Patient
{
    [Key]
    public int PatientId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateOnly BirthDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(200)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [InverseProperty(nameof(Appointment.Patient))]
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

