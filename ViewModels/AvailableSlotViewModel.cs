namespace Clinic_appointment.ViewModels;

public class AvailableSlotViewModel
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string DisplayText => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    public string Value => $"{StartTime:HH:mm}";
}
