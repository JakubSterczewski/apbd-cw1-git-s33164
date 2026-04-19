using C6.Enums;

namespace C6.DTOs;

public class UpdateAppointmentRequestDto
{
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
    public string? InternalNotes { get; set; }
}