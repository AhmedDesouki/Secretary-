namespace Clinic_appointment.DTOs;

public class CreateAppointmentDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationMinutes { get; set; }

     

}

