using Clinic_appointment.DTOs;
using Clinic_appointment.Services.Interfaces;
using Clinic_appointment.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_appointment.Controllers;

public class SecretaryController : Controller
{
    private readonly ISecretaryAppointmentService _secretaryAppointmentService;
    private readonly IPatientService _patientService;

    private const int DefaultSlotDurationMinutes = 30;
    private const int AppointmentsPageSize = 5;

    public SecretaryController(
        ISecretaryAppointmentService secretaryAppointmentService,
        IPatientService patientService)
    {
        _secretaryAppointmentService = secretaryAppointmentService;
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, string? phone = null)
    {
        if (page < 1) page = 1;
        var viewModel = await _secretaryAppointmentService.GetAppointmentsPagedAsync(page, AppointmentsPageSize, phone);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAppointment(CreateAppointmentViewModel model)
    {
        model = await _secretaryAppointmentService.PopulateCreateAppointmentAsync(
            model,
            DefaultSlotDurationMinutes);

        // This action is often used as a "reload" step (e.g., changing specialization).
        // Clear any querystring binding errors so we don't show validation on GET.
        ModelState.Clear();

        return View(model);
    }

    // Server-side patient lookup (no JavaScript):
    // Enter phone + click "Load" -> reloads CreateAppointment with patient info filled (if exists).
    [HttpGet]
    public async Task<IActionResult> LoadPatient(CreateAppointmentViewModel model)
    {
        var patient = await _patientService.GetByPhoneAsync(model.Patient.Phone);
        if (patient is not null)
        {
            model.Patient.Name = patient.Name;
            model.Patient.BirthDate = patient.BirthDate;
            model.Patient.Phone = patient.Phone;
            model.Patient.Email = patient.Email;
            model.Patient.Address = patient.Address;
        }
        else
        {
        
            TempData["PatientNotFound"] = "No patient found.";
        }

        // This is a "lookup" step, not the final submit. Clear validation errors
        // (ex: required Name/BirthDate) so the form can reload with populated data.
        ModelState.Clear();

        model = await _secretaryAppointmentService.PopulateCreateAppointmentAsync(
            model,
            DefaultSlotDurationMinutes);

        return View("CreateAppointment", model);
    }

    [HttpPost]
    [ActionName(nameof(CreateAppointment))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAppointmentPost(CreateAppointmentViewModel model) //from ViewModel --> request
    {
        if (!ModelState.IsValid)
        {
            model = await _secretaryAppointmentService.PopulateCreateAppointmentAsync(
                model,
                DefaultSlotDurationMinutes);

            return View(model);
        }

        try
        {
            await _secretaryAppointmentService.CreateAppointmentAsync(
                model,
                DefaultSlotDurationMinutes);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            model = await _secretaryAppointmentService.PopulateCreateAppointmentAsync(
                model,
                DefaultSlotDurationMinutes);

            return View(model);
        }

        TempData["SuccessMessage"] = "Appointment created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots(int doctorId, string date)
    {
        if (!DateOnly.TryParse(date, out var appointmentDate))
        {
            return BadRequest(new { error = "Invalid date format." });
        }

        IReadOnlyList<AvailableSlotJsonItem> result =
            await _secretaryAppointmentService.GetAvailableSlotsJsonAsync(
                doctorId,
                appointmentDate,
                DefaultSlotDurationMinutes);

        return Json(result);
    }

}
