namespace Clinic_appointment.DTOs;

public class CreatePatientDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
}

