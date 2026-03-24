using Clinic_appointment.Models;

namespace Clinic_appointment.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient?> GetByPhoneAsync(string phone);
}
