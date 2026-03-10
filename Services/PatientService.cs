using Clinic_appointment.Data;
using Clinic_appointment.DTOs;
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

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return new PatientDto
        {
            PatientId = patient.PatientId,
            Name = patient.Name,
            BirthDate = patient.BirthDate,
            Phone = patient.Phone,
            Email = patient.Email,
            Address = patient.Address
        };
    }

}
