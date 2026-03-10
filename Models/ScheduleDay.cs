using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_appointment.Models;

public class ScheduleDay
{
    [Key]
    public int ScheduleDayId { get; set; }

    [Required]
    public int ScheduleId { get; set; }

    [Required]
    [MaxLength(20)]
    public string DayOfWeek { get; set; } = string.Empty;

    [DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    [ForeignKey(nameof(ScheduleId))]
    [InverseProperty(nameof(Schedule.ScheduleDays))]
    public Schedule Schedule { get; set; } = null!;
}

