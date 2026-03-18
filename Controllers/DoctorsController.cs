using Clinic_appointment.Data;
using Clinic_appointment.Models;
using Clinic_appointment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Clinic_appointment.Controllers;

public class DoctorsController : Controller
{
    private readonly ClinicDbContext _context;

    public DoctorsController(ClinicDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Clinic)
            .Include(d => d.Schedule)
            .ThenInclude(s => s!.ScheduleDays)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();

        var model = new DoctorIndexViewModel
        {
            Doctors = doctors.Select(d => new DoctorRowViewModel
            {
                DoctorId = d.DoctorId,
                FullName = $"{d.FirstName} {d.LastName}",
                Specialization = d.Specialization,
                Email = d.Email,
                Phone = d.Phone,
                ClinicName = d.Clinic?.Name ?? string.Empty,
                ScheduleSummary = BuildScheduleSummary(d.Schedule)
            }).ToList()
        };

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
            return View(model);

        var clinicId = await _context.Clinics
            .OrderBy(c => c.ClinicId)
            .Select(c => c.ClinicId)
            .FirstAsync();

        var doctor = new Doctor
        {
            ClinicId = clinicId,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Specialization = model.Specialization.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim()
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        var schedule = new Schedule
        {
            DoctorId = doctor.DoctorId,
            EffFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
            EffTo = DateOnly.FromDateTime(DateTime.Today.AddYears(5))
        };
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        var selectedDays = model.Days
            .Where(d => d.IsSelected)
            .ToList();

        foreach (var day in selectedDays)
        {
            _context.ScheduleDays.Add(new ScheduleDay
            {
                ScheduleId = schedule.ScheduleId,
                DayOfWeek = day.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            });
        }

        await _context.SaveChangesAsync();

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

    private static string BuildScheduleSummary(Schedule? schedule)
    {
        if (schedule is null || schedule.ScheduleDays.Count == 0)
            return "No schedule";

        var dayOrder = new Dictionary<string, int>
        {
            { "sunday", 0 }, { "monday", 1 }, { "tuesday", 2 },
            { "wednesday", 3 }, { "thursday", 4 }, { "friday", 5 }, { "saturday", 6 }
        };

        var dayShort = new Dictionary<string, string>
        {
            { "sunday", "Sun" }, { "monday", "Mon" }, { "tuesday", "Tue" },
            { "wednesday", "Wed" }, { "thursday", "Thu" }, { "friday", "Fri" },
            { "saturday", "Sat" }
        };

        var parts = schedule.ScheduleDays
            .OrderBy(sd => dayOrder.GetValueOrDefault(sd.DayOfWeek.ToLowerInvariant(), 99))
            .Select(sd => $"{dayShort.GetValueOrDefault(sd.DayOfWeek.ToLowerInvariant(), sd.DayOfWeek)} {sd.StartTime:HH\\:mm}-{sd.EndTime:HH\\:mm}")
            .ToList();

        return string.Join(", ", parts);
    }
}

