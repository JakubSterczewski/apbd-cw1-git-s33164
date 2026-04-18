using C6.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace C6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public AppointmentsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        await _productRepository.TestConnectionAsync();

        return Ok();
    }
}