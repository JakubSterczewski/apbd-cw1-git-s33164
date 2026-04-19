using C6.DTOs;
using C6.Enums;
using C6.Exceptions;
using C6.Services;
using Microsoft.AspNetCore.Mvc;

namespace C6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string? status, string? patientLastName)
    {
        var appointments = await _appointmentService.GetAllAsync(status, patientLastName);
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(string id)
    {
        var appointment = await _appointmentService.GetDetailsAsync(id);
        return appointment is null
            ? NotFound(new ErrorResponseDto { Message = $"{id} not found." })
            : Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateAppointmentRequestDto dto)
    {
        try
        {
            await _appointmentService.AddAsync(dto);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponseDto { Message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new ErrorResponseDto { Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateAppointmentRequestDto dto)
    {
        try
        {
            await _appointmentService.UpdateAsync(id, dto);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponseDto { Message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new ErrorResponseDto { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponseDto { Message = ex.Message });
        }
    }
}