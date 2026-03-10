using Clinic_appointment.Data;
using Clinic_appointment.DTOs;
using Clinic_appointment.Models;
using Clinic_appointment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Services;

public class DoctorService : IDoctorService
{
    private readonly ClinicDbContext _context;

    public DoctorService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DoctorDto>> GetAllAsync()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Clinic)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();

        return doctors
            .Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                ClinicId = d.ClinicId,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Specialization = d.Specialization,
                Email = d.Email,
                Phone = d.Phone,
                ClinicName = d.Clinic?.Name ?? string.Empty
            })
            .ToList();
    }
}
