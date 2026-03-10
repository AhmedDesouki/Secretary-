using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Models;

[Index(nameof(DoctorId), nameof(AppointmentDate))]
public class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [DataType(DataType.Date)]
    public DateOnly AppointmentDate { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    [Range(1, 480)]
    public int DurationMinutes { get; set; } = 30;

    [ForeignKey(nameof(PatientId))]
    [InverseProperty(nameof(Models.Patient.Appointments))]
    public Patient Patient { get; set; } = null!;

    [ForeignKey(nameof(DoctorId))]
    [InverseProperty(nameof(Models.Doctor.Appointments))]
    public Doctor Doctor { get; set; } = null!;
}

