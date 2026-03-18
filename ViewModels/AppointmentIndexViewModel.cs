namespace Clinic_appointment.ViewModels;

public class AppointmentIndexViewModel
{
    public IReadOnlyList<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string? PhoneFilter { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}
