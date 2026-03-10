using Clinic_appointment.DTOs;
using Clinic_appointment.Services.Interfaces;
using Clinic_appointment.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_appointment.Controllers;

public class SecretaryController : Controller
{
    private readonly ISecretaryAppointmentService _secretaryAppointmentService;

    private const int DefaultSlotDurationMinutes = 30;

    public SecretaryController(
        ISecretaryAppointmentService secretaryAppointmentService)
    {
        _secretaryAppointmentService = secretaryAppointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        IReadOnlyList<AppointmentViewModel> viewModels =
            await _secretaryAppointmentService.GetAppointmentsAsync();

        return View(viewModels);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAppointment()
    {
        CreateAppointmentViewModel viewModel = new();
        viewModel = await _secretaryAppointmentService.PopulateCreateAppointmentAsync(
            viewModel,
            DefaultSlotDurationMinutes);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAppointment(CreateAppointmentViewModel model)
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
