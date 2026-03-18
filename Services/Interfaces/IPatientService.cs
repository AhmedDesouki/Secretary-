using Clinic_appointment.DTOs;

namespace Clinic_appointment.Services.Interfaces;

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<PatientDto?> GetByPhoneAsync(string phone);
}
