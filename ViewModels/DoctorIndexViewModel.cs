namespace Clinic_appointment.ViewModels;

public class DoctorIndexViewModel
{
    public List<DoctorRowViewModel> Doctors { get; set; } = new();
}

public class DoctorRowViewModel
{
    public int DoctorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public string ScheduleSummary { get; set; } = string.Empty;
}

