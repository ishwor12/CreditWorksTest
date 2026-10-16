using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.IServices;

namespace VehicleManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        [HttpGet]
        public async Task<ActionResult<List<VehicleDto>>> GetAll([FromQuery] string? sortBy, [FromQuery] bool descending = false)
        {
            var vehicles = await _vehicleService.GetAllAsync(sortBy, descending);
            return Ok(vehicles);
        }
        [HttpPost]
        public async Task<ActionResult<VehicleDto>> Create(CreateVehicleRequest request)
        {
            var vehicle = await _vehicleService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created, vehicle);
        }
    }
}
