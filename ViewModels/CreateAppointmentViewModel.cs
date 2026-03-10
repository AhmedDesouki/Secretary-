using System.ComponentModel.DataAnnotations;

namespace Clinic_appointment.ViewModels;

public class CreateAppointmentViewModel
{
    public PatientViewModel Patient { get; set; } = new();

    [Required(ErrorMessage = "Please select a doctor")]
    [Display(Name = "Doctor")]
    public int? SelectedDoctorId { get; set; }

    [Required(ErrorMessage = "Please select an appointment date")]
    [DataType(DataType.Date)]
    [Display(Name = "Appointment Date")]
    public DateOnly AppointmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Please select a time slot")]
    [Display(Name = "Time Slot")]
    public string? SelectedTimeSlot { get; set; }

    public List<DoctorOptionViewModel> Doctors { get; set; } = new();
    public List<AvailableSlotViewModel> AvailableSlots { get; set; } = new();
}

public class DoctorOptionViewModel
{
    public int DoctorId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
}
