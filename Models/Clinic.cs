using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_appointment.Models;

public class Clinic
{
    [Key]
    public int ClinicId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [InverseProperty(nameof(Doctor.Clinic))]
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}

