using Clinic_appointment.Data;
using Clinic_appointment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Services;

public class ScheduleService : IScheduleService
{
    private readonly ClinicDbContext _context;

    public ScheduleService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<(TimeOnly StartTime, TimeOnly EndTime)>> GetWorkingHoursAsync(int doctorId, DateOnly date)
    {
        var dayOfWeek = date.DayOfWeek.ToString();

        // Check DoctorAvailability first - specific date overrides schedule
        var availability = await _context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId && a.AvailabilityDate == date)
            .FirstOrDefaultAsync();

        if (availability != null)
        {
            if (!availability.IsAvailable)
                return [];

            return [(availability.StartTime, availability.EndTime)];
        }

        // Use schedule - find effective schedule for the date
        var schedule = await _context.Schedules
            .Include(s => s.ScheduleDays)
            .Where(s => s.DoctorId == doctorId && s.EffFrom <= date && s.EffTo >= date)
            .FirstOrDefaultAsync();

        if (schedule == null)
            return [];

        var scheduleDays = schedule.ScheduleDays
            .Where(sd => sd.DayOfWeek.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase))
            .Select(sd => (sd.StartTime, sd.EndTime))
            .ToList();

        return scheduleDays;
    }
}
