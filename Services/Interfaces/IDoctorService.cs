using Clinic_appointment.DTOs;

namespace Clinic_appointment.Services.Interfaces;

public interface IDoctorService
{
    Task<IReadOnlyList<DoctorDto>> GetAllAsync();
}
