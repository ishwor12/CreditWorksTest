namespace VehicleManagement.Application.DTOs;

public class CategoryInputDto
{
    // Null means a new category.
    public int? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string IconKey { get; set; } = string.Empty;

    public decimal MinWeightKg { get; set; }

    // Null means no upper limit.
    public decimal? MaxWeightKg { get; set; }
}