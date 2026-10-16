using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.IServices;

namespace VehicleManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(SaveCategoryRequest request)
        {
            var category = await _categoryService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created, category);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, SaveCategoryRequest request)
        {
            var category = await _categoryService.UpdateAsync(id, request);
            return Ok(category);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
