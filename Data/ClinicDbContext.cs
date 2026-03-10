using Clinic_appointment.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_appointment.Data;

public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ScheduleDay> ScheduleDays => Set<ScheduleDay>();
    public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyFluentApiConfiguration(modelBuilder);
    }

    public static async Task InitializeAsync(ClinicDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Clinics.AnyAsync())
            return;

        await SeedAsync(context);
    }

    private static async Task SeedAsync(ClinicDbContext context)
    {
        var clinic = new Clinic
        {
            Name = "Main Clinic",
            Address = "123 Medical Center Dr",
            Phone = "+1-555-0100"
        };
        context.Clinics.Add(clinic);
        await context.SaveChangesAsync();

        var doctor = new Doctor
        {
            ClinicId = clinic.ClinicId,
            FirstName = "Ahmed",
            LastName = "Mohamed",
            Specialization = "General Practice",
            Email = "ahmed.Mohamed@clinic.com",
            Phone = "+201043553"
        };
        context.Doctors.Add(doctor);
        await context.SaveChangesAsync();

        var schedule = new Schedule
        {
            DoctorId = doctor.DoctorId,
            EffFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
            EffTo = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
        };
        context.Schedules.Add(schedule);
        await context.SaveChangesAsync();

        var days = new[] { "Sunday","Monday", "Tuesday", "Wednesday", "Thursday" };
        foreach (var day in days)
        {
            context.ScheduleDays.Add(new ScheduleDay
            {
                ScheduleId = schedule.ScheduleId,
                DayOfWeek = day,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(17, 0)
            });
        }

        context.Doctors.Add(new Doctor
        {
            ClinicId = clinic.ClinicId,
            FirstName = "Hassan",
            LastName = "Ahmed",
            Specialization = "Cardiology",
            Email = "Hassan.ahmed@clinic.com",
            Phone = "+1-555-0102"
        });
        await context.SaveChangesAsync();

        var doctor2 = await context.Doctors.FirstAsync(d => d.LastName == "Ahmed");
        context.Schedules.Add(new Schedule
        {
            DoctorId = doctor2.DoctorId,
            EffFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
            EffTo = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
        });
        await context.SaveChangesAsync();

        var schedule2 = await context.Schedules.FirstAsync(s => s.DoctorId == doctor2.DoctorId);
        foreach (var day in days)
        {
            context.ScheduleDays.Add(new ScheduleDay
            {
                ScheduleId = schedule2.ScheduleId,
                DayOfWeek = day,
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(16, 0)
            });
        }

        await context.SaveChangesAsync();

        context.DoctorAvailabilities.Add(new DoctorAvailability
        {
            DoctorId = doctor.DoctorId,
            AvailabilityDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            Reason = "Medical Conference"
        });
        context.DoctorAvailabilities.Add(new DoctorAvailability
        {
            DoctorId = doctor2.DoctorId,
            AvailabilityDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            Reason = "Personal Leave"
        });
        await context.SaveChangesAsync();
    }

    private static void ApplyFluentApiConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.ClinicId);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Specialization).HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(e => e.Clinic)
                .WithMany(c => c.Doctors)
                .HasForeignKey(e => e.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Schedule)
                .WithOne(s => s.Doctor)
                .HasForeignKey<Schedule>(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);

            entity.HasOne(e => e.Doctor)
                .WithOne(d => d.Schedule)
                .HasForeignKey<Schedule>(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ScheduleDays)
                .WithOne(sd => sd.Schedule)
                .HasForeignKey(sd => sd.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ScheduleDay>(entity =>
        {
            entity.HasKey(e => e.ScheduleDayId);
            entity.Property(e => e.DayOfWeek).HasMaxLength(20);
        });

        modelBuilder.Entity<DoctorAvailability>(entity =>
        {
            entity.HasKey(e => e.AvailabilityId);
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Availabilities)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DoctorId, e.AvailabilityDate });
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.DoctorId, e.AppointmentDate });
        });
    }
}
