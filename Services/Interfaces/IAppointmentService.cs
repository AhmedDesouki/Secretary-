using Clinic_appointment.DTOs;

namespace Clinic_appointment.Services.Interfaces;

public interface IAppointmentService
{
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(int doctorId, DateOnly date, int slotDurationMinutes = 30);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<IReadOnlyList<AppointmentDto>> GetAllAppointmentsAsync();
}
