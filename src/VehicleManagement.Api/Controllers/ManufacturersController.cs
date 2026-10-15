using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.IServices;

namespace VehicleManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ManufacturerDto>>> GetAll()
        {
            var manufacturers = await _manufacturerService.GetAllAsync();
            return Ok(manufacturers);
        }
    }
}
