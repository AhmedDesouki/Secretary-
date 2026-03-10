using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_appointment.Models;

public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [DataType(DataType.Date)]
    public DateOnly EffFrom { get; set; }

    [DataType(DataType.Date)]
    public DateOnly EffTo { get; set; }

    [ForeignKey(nameof(DoctorId))]
    [InverseProperty(nameof(Doctor.Schedule))]
    public Doctor Doctor { get; set; } = null!;

    [InverseProperty(nameof(ScheduleDay.Schedule))]
    public ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
}

