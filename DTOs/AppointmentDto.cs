namespace Clinic_appointment.DTOs;

public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationMinutes { get; set; }

    public string PatientName { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;
    public string DoctorFirstName { get; set; } = string.Empty;
    public string DoctorLastName { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
}

