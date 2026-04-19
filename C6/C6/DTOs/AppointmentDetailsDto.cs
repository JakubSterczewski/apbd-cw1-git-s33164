namespace C6.DTOs;

public class AppointmentDetailsDto : AppointmentListDto
{
    public string DoctorFullName { get; set; } = string.Empty;
    public string InternalNotes { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}