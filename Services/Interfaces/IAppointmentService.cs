using Clinic_appointment.Models;

namespace Clinic_appointment.Services.Interfaces;

public interface IAppointmentService
{
    Task<IReadOnlyList<(TimeOnly StartTime, TimeOnly EndTime)>> GetAvailableSlotsAsync(int doctorId, DateOnly date, int slotDurationMinutes = 30);
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<IReadOnlyList<Appointment>> GetAllAppointmentsAsync();
}
