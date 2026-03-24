using Clinic_appointment.Data;
using Clinic_appointment.Models;
using Clinic_appointment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Services;

public class PatientService : IPatientService
{
    private readonly ClinicDbContext _context;

    public PatientService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> CreateAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return patient;
    }

    public async Task<Patient?> GetByPhoneAsync(string phone)
    {
        var trimmed = phone?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return null;
        }

        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Phone == trimmed);
    }
}
