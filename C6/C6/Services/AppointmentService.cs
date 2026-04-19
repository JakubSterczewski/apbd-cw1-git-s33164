using C6.DTOs;
using C6.Enums;
using C6.Exceptions;
using Microsoft.Data.SqlClient;

namespace C6.Services;

public class AppointmentService : IAppointmentService
{
    private readonly string _connectionString;

    public AppointmentService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException(
                                "Brak ConnectionString 'DefaultConnection' w konfiguracji!");
    }

    public async Task<IEnumerable<AppointmentListDto>> GetAllAsync(string? status = null,
        string? patientLastName = null)
    {
        var result = new List<AppointmentListDto>();
        await using var connection = new SqlConnection(_connectionString);

        var commandString =
            "SELECT IdAppointment, AppointmentDate, Status, Reason, FirstName + ' ' + LastName AS FullName, Email FROM Appointments a JOIN Patients p ON a.IdPatient = p.IdPatient";
        var optionalConditions = new List<string>();
        if (status is not null)
        {
            if (!Enum.IsDefined(typeof(AppointmentStatus), status))
                throw new BadRequestException($"Invalid status: {status}.");
            optionalConditions.Add("a.Status = @Status");
        }

        if (patientLastName is not null)
            optionalConditions.Add("p.LastName LIKE @PatientLastName");
        if (optionalConditions.Count > 0)
            commandString += " WHERE " + string.Join(" AND ", optionalConditions);

        var command = new SqlCommand(commandString, connection);
        if (status is not null)
            command.Parameters.AddWithValue("@Status", status);
        if (patientLastName is not null)
            command.Parameters.AddWithValue("@PatientLastName", $"{patientLastName}");

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(MapToAppointmentListDto(reader));
        return result;
    }

    public async Task<AppointmentDetailsDto?> GetDetailsAsync(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        var command = new SqlCommand(
            "SELECT IdAppointment, AppointmentDate, Status, Reason, p.FirstName + ' ' + p.LastName AS FullName, d.FirstName + ' ' + d.LastName AS DoctorFullName, Email, InternalNotes, CreatedAt FROM Appointments a JOIN Patients p ON a.IdPatient = p.IdPatient JOIN Doctors d ON d.IdDoctor = a.IdDoctor WHERE a.IdAppointment = @id",
            connection);
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapToAppointmentDetailsDto(reader) : null;
    }

    public async Task AddAsync(CreateAppointmentRequestDto dto)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await CheckDoctorExistsAsync(connection, dto.IdDoctor);
        await CheckPatientExistsAsync(connection, dto.IdPatient);

        if (dto.AppointmentDate < DateTime.Now) throw new ConflictException("Cannot add an appointment in the past.");

        if (string.IsNullOrEmpty(dto.InternalNotes) || dto.InternalNotes.Length > 250)
            throw new ConflictException("InternalNotes cannot be empty and must have at most 250 characters.");

        var checkConflict = new SqlCommand(
            "SELECT COUNT(*) FROM Appointments WHERE IdDoctor=@IdDoctor AND AppointmentDate=@AppointmentDate",
            connection);
        checkConflict.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
        checkConflict.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
        if ((int)await checkConflict.ExecuteScalarAsync()! > 0)
            throw new ConflictException($"Doctor {dto.IdDoctor} already has an appointment at that time.");

        var command = new SqlCommand(
            "INSERT INTO Appointments (AppointmentDate, Status, Reason, IdPatient, IdDoctor, InternalNotes) VALUES (@AppointmentDate, @Status, @Reason, @IdPatient, @IdDoctor, @InternalNotes)",
            connection);
        command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
        command.Parameters.AddWithValue("@Status", "Scheduled");
        command.Parameters.AddWithValue("@Reason", dto.Reason);
        command.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
        command.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
        command.Parameters.AddWithValue("@InternalNotes", (object?)dto.InternalNotes ?? DBNull.Value);
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(string id, UpdateAppointmentRequestDto dto)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await CheckDoctorExistsAsync(connection, dto.IdDoctor);
        await CheckPatientExistsAsync(connection, dto.IdPatient);

        if (!Enum.IsDefined(typeof(AppointmentStatus), dto.Status))
            throw new BadRequestException($"Invalid status: {dto.Status}.");

        var checkStatusCommand = new SqlCommand(
            "SELECT Status, AppointmentDate FROM Appointments WHERE IdAppointment=@IdAppointment",
            connection);
        checkStatusCommand.Parameters.AddWithValue("@IdAppointment", id);

        await using var reader = await checkStatusCommand.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            if (reader.GetDateTime(reader.GetOrdinal("AppointmentDate")).Equals(dto.AppointmentDate))
            {
                if (Enum.Parse<AppointmentStatus>(reader.GetString(reader.GetOrdinal("Status"))) ==
                    AppointmentStatus.Completed)
                    throw new ConflictException(
                        $"Cannot change date of the appointment {id}, beacause it is already completed.");
                var checkAvailabilityCommand = new SqlCommand(
                    "SELECT COUNT(*) FROM Appointments WHERE IdAppointment!=@IdAppointment AND  IdDoctor=@IdDoctor AND AppointmentDate=@AppointmentDate",
                    connection);
                checkAvailabilityCommand.Parameters.AddWithValue("@IdAppointment", id);
                checkAvailabilityCommand.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
                checkAvailabilityCommand.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);

                var count = (int?)await checkAvailabilityCommand.ExecuteScalarAsync();

                if (count > 0) throw new ConflictException($"Doctor {id} has appointments at that time.");
            }

        var command = new SqlCommand(
            "UPDATE Appointments SET AppointmentDate=@AppointmentDate, Status=@Status, Reason=@Reason, IdPatient=@IdPatient, IdDoctor=@IdDoctor, InternalNotes=@InternalNotes WHERE IdAppointment=@IdAppointment",
            connection);
        command.Parameters.AddWithValue("@IdAppointment", id);
        command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
        command.Parameters.AddWithValue("@Status", dto.Status.ToString());
        command.Parameters.AddWithValue("@Reason", dto.Reason);
        command.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
        command.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
        command.Parameters.AddWithValue("@InternalNotes", (object?)dto.InternalNotes ?? DBNull.Value);
        var rows = await command.ExecuteNonQueryAsync();
        if (rows == 0)
            throw new NotFoundException($"Appointment {id} not found.");
    }

    public async Task DeleteAsync(string id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var selectCommand = new SqlCommand("SELECT Status FROM Appointments WHERE IdAppointment=@IdAppointment",
            connection);
        selectCommand.Parameters.AddWithValue("@IdAppointment", id);
        var status = (string?)await selectCommand.ExecuteScalarAsync();
        if (status is null)
            throw new NotFoundException($"{id} not found.");
        if (Enum.Parse<AppointmentStatus>(status) == AppointmentStatus.Completed)
            throw new ConflictException($"{id} is already completed.");

        var deleteCommand = new SqlCommand("DELETE FROM Appointments WHERE IdAppointment=@IdAppointment", connection);
        deleteCommand.Parameters.AddWithValue("@IdAppointment", id);
        await deleteCommand.ExecuteNonQueryAsync();
    }

    private static async Task CheckDoctorExistsAsync(SqlConnection connection, int idDoctor)
    {
        var command = new SqlCommand("SELECT COUNT(*) FROM Doctors WHERE IdDoctor=@IdDoctor AND IsActive=1",
            connection);
        command.Parameters.AddWithValue("@IdDoctor", idDoctor);
        if ((int)await command.ExecuteScalarAsync()! == 0)
            throw new NotFoundException($"Doctor {idDoctor} not found.");
    }

    private static async Task CheckPatientExistsAsync(SqlConnection connection, int idPatient)
    {
        var command = new SqlCommand("SELECT COUNT(*) FROM Patients WHERE IdPatient=@IdPatient AND IsActive=1",
            connection);
        command.Parameters.AddWithValue("@IdPatient", idPatient);
        if ((int)await command.ExecuteScalarAsync()! == 0)
            throw new NotFoundException($"Patient {idPatient} not found.");
    }

    private static AppointmentListDto MapToAppointmentListDto(SqlDataReader reader)
    {
        return new AppointmentListDto
        {
            IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
            AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            Reason = reader.GetString(reader.GetOrdinal("Reason")),
            PatientFullName = reader.GetString(reader.GetOrdinal("FullName")),
            PatientEmail = reader.GetString(reader.GetOrdinal("Email"))
        };
    }

    private static AppointmentDetailsDto MapToAppointmentDetailsDto(SqlDataReader reader)
    {
        return new AppointmentDetailsDto
        {
            IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
            AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            Reason = reader.GetString(reader.GetOrdinal("Reason")),
            PatientFullName = reader.GetString(reader.GetOrdinal("FullName")),
            PatientEmail = reader.GetString(reader.GetOrdinal("Email")),
            DoctorFullName = reader.GetString(reader.GetOrdinal("DoctorFullName")),
            InternalNotes = reader.IsDBNull(reader.GetOrdinal("InternalNotes"))
                ? null
                : reader.GetString(reader.GetOrdinal("InternalNotes")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}