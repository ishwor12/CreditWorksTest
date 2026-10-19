using Microsoft.EntityFrameworkCore;
using VehicleManagement.Application.Abstractions;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.Exceptions;
using VehicleManagement.Application.IServices;
using VehicleManagement.Application.Validation;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Validations;
namespace VehicleManagement.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IAppDbContext _db;

    public CategoryService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _db.VehicleCategories.OrderBy(c => c.MinWeightKg).ToListAsync();
        return categories.Select(ToDto).ToList();
    }

    public async Task<List<CategoryDto>> SaveAllAsync(List<CategoryInputDto> inputs)
    {
        // 1. Check the names and icons.
        var errors = CategoryInputValidator.Validate(inputs);

        // 2. Build the proposed categories as plain objects. Nothing is saved yet.
        var proposed = new List<VehicleCategory>();
        foreach (var input in inputs)
        {
            proposed.Add(new VehicleCategory
            {
                Name = (input.Name ?? string.Empty).Trim(),
                IconKey = (input.IconKey ?? string.Empty).Trim(),
                MinWeightKg = input.MinWeightKg,
                MaxWeightKg = input.MaxWeightKg
            });
        }

        // 3. Check the whole set of ranges: no gaps, no overlaps, every weight covered.
        if (inputs.Count > 0)
        {
            errors.AddRange(CategoryRangeValidator.Validate(proposed));
        }

        if (errors.Count > 0)
        {
            throw new ValidationFailedException(errors);
        }

        // 4. Everything is valid, so apply the changes.
        var existing = await _db.VehicleCategories.ToListAsync();

        foreach (var input in inputs)
        {
            if (input.Id != null && !existing.Any(c => c.Id == input.Id))
            {
                throw new ValidationFailedException($"Category {input.Id} no longer exists. Reload the page and try again.");
            }
        }

        // Categories that are no longer in the list are deleted.
        foreach (var category in existing)
        {
            if (!inputs.Any(i => i.Id == category.Id))
            {
                _db.VehicleCategories.Remove(category);
            }
        }

        for (var i = 0; i < inputs.Count; i++)
        {
            var values = proposed[i];

            if (inputs[i].Id == null)
            {
                _db.VehicleCategories.Add(values);
            }
            else
            {
                var category = existing.First(c => c.Id == inputs[i].Id);
                category.Name = values.Name;
                category.IconKey = values.IconKey;
                category.MinWeightKg = values.MinWeightKg;
                category.MaxWeightKg = values.MaxWeightKg;
            }
        }

        // One SaveChanges is one database transaction, so everything succeeds or fails together.
        await _db.SaveChangesAsync();

        return await GetAllAsync();
    }

    private static CategoryDto ToDto(VehicleCategory category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            IconKey = category.IconKey,
            MinWeightKg = category.MinWeightKg,
            MaxWeightKg = category.MaxWeightKg
        };
    }
}