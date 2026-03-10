using Clinic_appointment.ViewModels;

namespace Clinic_appointment.Services.Interfaces;

public interface ISecretaryAppointmentService
{
    Task<IReadOnlyList<AppointmentViewModel>> GetAppointmentsAsync();
    Task<CreateAppointmentViewModel> PopulateCreateAppointmentAsync(CreateAppointmentViewModel model, int slotDurationMinutes);
    Task CreateAppointmentAsync(CreateAppointmentViewModel model, int slotDurationMinutes);
    Task<IReadOnlyList<AvailableSlotJsonItem>> GetAvailableSlotsJsonAsync(int doctorId, DateOnly appointmentDate, int slotDurationMinutes);
}

public sealed record AvailableSlotJsonItem(string value, string display);

