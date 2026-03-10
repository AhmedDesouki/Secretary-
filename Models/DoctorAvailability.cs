using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Models;

[Index(nameof(DoctorId), nameof(AvailabilityDate))]
public class DoctorAvailability
{
    [Key]
    public int AvailabilityId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [DataType(DataType.Date)]
    public DateOnly AvailabilityDate { get; set; }

    public bool IsAvailable { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    [ForeignKey(nameof(DoctorId))]
    [InverseProperty(nameof(Doctor.Availabilities))]
    public Doctor Doctor { get; set; } = null!;
}

