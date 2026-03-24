using Clinic_appointment.Data;
using Clinic_appointment.Models;
using Clinic_appointment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ClinicDbContext _context;
    private readonly IScheduleService _scheduleService;

    public AppointmentService(ClinicDbContext context, IScheduleService scheduleService)
    {
        _context = context;
        _scheduleService = scheduleService;
    }

    public async Task<IReadOnlyList<(TimeOnly StartTime, TimeOnly EndTime)>> GetAvailableSlotsAsync(int doctorId, DateOnly date, int slotDurationMinutes = 30)
    {
        // Prevent selecting past dates
        if (date < DateOnly.FromDateTime(DateTime.Today))
            throw new InvalidOperationException("Cannot book appointments for past dates.");

        var workingHours = await _scheduleService.GetWorkingHoursAsync(doctorId, date);
     
        if (workingHours.Count == 0)
            return [];

        var existingAppointments = await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
            .Select(a => new { a.StartTime, a.EndTime })
            .ToListAsync();

        var slots = new List<(TimeOnly StartTime, TimeOnly EndTime)>();
        var slotDuration = TimeSpan.FromMinutes(slotDurationMinutes);

        foreach (var (startTime, endTime) in workingHours)
        {
            var current = startTime;
            while (current.Add(slotDuration) <= endTime)
            {
                var slotEnd = TimeOnly.FromTimeSpan(current.ToTimeSpan().Add(slotDuration));
                var overlaps = existingAppointments.Any(a =>
                    current < a.EndTime && slotEnd > a.StartTime);

                if (!overlaps)
                    slots.Add((current, slotEnd));

                current = slotEnd;
            }
        }

        return slots.OrderBy(s => s.StartTime).ToList();
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return appointment;
    }
    //max 1 w 
    //client -->
    //server --> 

    public async Task<IReadOnlyList<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Clinic)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }
}
