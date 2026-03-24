using Clinic_appointment.Models;
using Clinic_appointment.ViewModels;

namespace Clinic_appointment.Services.Interfaces;

public interface IDoctorService
{
    Task<IReadOnlyList<Doctor>> GetAllAsync();
    Task<DoctorIndexViewModel> GetDoctorsPagedAsync();
    Task<CreateDoctorViewModel> CreateAsync(CreateDoctorViewModel model);
}
