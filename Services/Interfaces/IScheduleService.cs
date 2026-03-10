namespace Clinic_appointment.Services.Interfaces;

public interface IScheduleService
{
    Task<IReadOnlyList<(TimeOnly StartTime, TimeOnly EndTime)>> GetWorkingHoursAsync(int doctorId, DateOnly date);
}
