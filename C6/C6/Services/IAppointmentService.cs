using C6.DTOs;
using C6.Enums;

namespace C6.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentListDto>> GetAllAsync(string? status = null, string? patientLastName = null);
    Task<AppointmentDetailsDto?> GetDetailsAsync(string id);
    Task AddAsync(CreateAppointmentRequestDto dto);
    Task UpdateAsync(string id, UpdateAppointmentRequestDto dto);
    Task DeleteAsync(string id);
}