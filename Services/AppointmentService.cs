using Clinic_appointment.Data;
using Clinic_appointment.DTOs;
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

    public async Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(int doctorId, DateOnly date, int slotDurationMinutes = 30)
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

        var slots = new List<TimeSlotDto>();
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
                    slots.Add(new TimeSlotDto(current, slotEnd));

                current = slotEnd;
            }
        }

        return slots.OrderBy(s => s.StartTime).ToList();
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var endTime = dto.StartTime.Add(TimeSpan.FromMinutes(dto.DurationMinutes));
        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            AppointmentDate = dto.AppointmentDate,
            StartTime = dto.StartTime,
            EndTime = endTime,
            DurationMinutes = dto.DurationMinutes
        };
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return new AppointmentDto
        {
            AppointmentId = appointment.AppointmentId,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            DurationMinutes = appointment.DurationMinutes
        };
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetAllAppointmentsAsync()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Clinic)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

        return appointments
            .Select(a => new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                DurationMinutes = a.DurationMinutes,
                PatientName = a.Patient.Name,
                DoctorFirstName = a.Doctor.FirstName,
                DoctorLastName = a.Doctor.LastName,
                ClinicName = a.Doctor.Clinic?.Name ?? string.Empty
            })
            .ToList();
    }
}
