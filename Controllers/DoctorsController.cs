using Clinic_appointment.Data;
using Clinic_appointment.Models;
using Clinic_appointment.Services;
using Clinic_appointment.Services.Interfaces;
using Clinic_appointment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Clinic_appointment.Controllers;

public class DoctorsController : Controller
{
    //private readonly ClinicDbContext _context;
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await _doctorService.GetDoctorsPagedAsync();
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDoctorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDoctorViewModel model)
    {
        ValidateSchedule(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        model = await _doctorService.CreateAsync(model);
        TempData["SuccessMessage"] = "Doctor created successfully.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateSchedule(CreateDoctorViewModel model)
    {
        if (model.StartTime >= model.EndTime)
        {
            ModelState.AddModelError(nameof(model.EndTime), "End time must be after start time.");
        }

        if (model.Days is null || model.Days.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Please configure the schedule days.");
            return;
        }
        var workingStart = new TimeOnly(9, 0);  // 9:00 AM
        var workingEnd = new TimeOnly(16, 0);   // 4:00 PM
        // Validate working hours for StartTime
        if (model.StartTime < workingStart || model.StartTime > workingEnd)
        {
            ModelState.AddModelError(nameof(model.StartTime),
                "Start time must be between 9:00 AM and 4:00 PM.");
        }

        // Validate working hours for EndTime
        if (model.EndTime < workingStart || model.EndTime > workingEnd)
        {
            ModelState.AddModelError(nameof(model.EndTime),
                "End time must be between 9:00 AM and 4:00 PM.");
        }

        var selected = model.Days.Where(d => d.IsSelected).ToList();
        if (selected.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Please select at least one working day.");
            return;
        }


    }

   
}

