namespace Clinic_appointment.DTOs;

public class DoctorDto
{
    public int DoctorId { get; set; }
    public int ClinicId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
}

