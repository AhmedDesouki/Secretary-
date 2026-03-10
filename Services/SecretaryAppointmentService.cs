using Clinic_appointment.DTOs;
using Clinic_appointment.Services.Interfaces;
using Clinic_appointment.ViewModels;

namespace Clinic_appointment.Services;

public class SecretaryAppointmentService : ISecretaryAppointmentService
{
    private readonly IPatientService _patientService;
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;

    public SecretaryAppointmentService(
        IPatientService patientService,
        IAppointmentService appointmentService,
        IDoctorService doctorService)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;
        _doctorService = doctorService;
    }

    public async Task<IReadOnlyList<AppointmentViewModel>> GetAppointmentsAsync()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();

        return appointments
            .Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientName = a.PatientName,
                DoctorName = $"{a.DoctorFirstName} {a.DoctorLastName}",
                ClinicName = a.ClinicName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                DurationMinutes = a.DurationMinutes
            })
            .ToList();
    }

    public async Task<CreateAppointmentViewModel> PopulateCreateAppointmentAsync(
        CreateAppointmentViewModel model,
        int slotDurationMinutes)
    {
        var doctors = await _doctorService.GetAllAsync();
        model.Doctors = doctors
            .Select(d => new DoctorOptionViewModel
            {
                DoctorId = d.DoctorId,
                DisplayName = $"{d.FirstName} {d.LastName} ({d.Specialization})",
                Specialization = d.Specialization
            })
            .ToList();

        model.AvailableSlots = [];
        if ( model.SelectedDoctorId > 0)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(
                model.SelectedDoctorId.Value,
                model.AppointmentDate,
                slotDurationMinutes);

            model.AvailableSlots = slots
                .Select(s => new AvailableSlotViewModel
                {
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                })
                .ToList();
        }

        return model;
    }

    public async Task CreateAppointmentAsync(CreateAppointmentViewModel model, int slotDurationMinutes)
    {
        // Parse the time slot
        // Format is controlled by UI dropdown, so this should always succeed
        var startTime = TimeOnly.Parse(model.SelectedTimeSlot);

        // Ensure the selected slot is currently available
        var availableSlots = await _appointmentService.GetAvailableSlotsAsync(
            model.SelectedDoctorId.Value,
            model.AppointmentDate,
            slotDurationMinutes);

        if (!availableSlots.Any(s => s.StartTime == startTime))
        {
            throw new InvalidOperationException("Selected slot is no longer available. Please choose another slot.");
        }

        var patient = await _patientService.CreateAsync(new CreatePatientDto
        {
            Name = model.Patient.Name,
            BirthDate = model.Patient.BirthDate,
            Phone = model.Patient.Phone,
            Email = model.Patient.Email,
            Address = model.Patient.Address
        });

        await _appointmentService.CreateAsync(new CreateAppointmentDto
        {
            PatientId = patient.PatientId,
            DoctorId = model.SelectedDoctorId.Value,
            AppointmentDate = model.AppointmentDate,
            StartTime = startTime,
            DurationMinutes = slotDurationMinutes
        });
    }

    public async Task<IReadOnlyList<AvailableSlotJsonItem>> GetAvailableSlotsJsonAsync(
        int doctorId,
        DateOnly appointmentDate,
        int slotDurationMinutes)
    {
        var slots = await _appointmentService.GetAvailableSlotsAsync(
            doctorId,
            appointmentDate,
            slotDurationMinutes);

        return slots
            .Select(s => new AvailableSlotJsonItem(
                value: s.StartTime.ToString("HH:mm"),
                display: $"{s.StartTime:hh\\:mm} - {s.EndTime:hh\\:mm}"))
            .ToList();
    }
}

