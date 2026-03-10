using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_appointment.Models;

public class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    [Required]
    public int ClinicId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
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

    [ForeignKey(nameof(ClinicId))]
    [InverseProperty(nameof(Models.Clinic.Doctors))]
    public Clinic Clinic { get; set; } = null!;

    [InverseProperty(nameof(Schedule.Doctor))]
    public Schedule? Schedule { get; set; }

    [InverseProperty(nameof(DoctorAvailability.Doctor))]
    public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();

    [InverseProperty(nameof(Appointment.Doctor))]
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

