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

        [HttpPost]
        public async Task<ActionResult<VehicleDto>> Create(CreateVehicleRequest request)
        {
            var vehicle = await _vehicleService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created, vehicle);
        }
    }
}
