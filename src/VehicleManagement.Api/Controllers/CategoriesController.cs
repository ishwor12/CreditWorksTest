using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.IServices;

namespace VehicleManagement.Api.Controllers;

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

    // Replaces the whole category list. The ranges are validated as one set.
    [HttpPut]
    public async Task<ActionResult<List<CategoryDto>>> SaveAll(List<CategoryInputDto> categories)
    {
        var saved = await _categoryService.SaveAllAsync(categories);
        return Ok(saved);
    }
}