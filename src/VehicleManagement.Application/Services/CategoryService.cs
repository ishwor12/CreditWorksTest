using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.Abstractions;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.Exceptions;
using VehicleManagement.Application.IServices;
using VehicleManagement.Application.Validation;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Validations;

namespace VehicleManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IAppDbContext _dbcontext;

        public CategoryService(IAppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _dbcontext.VehicleCategories
                .OrderBy(c => c.MinWeightKg)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconKey = c.IconKey,
                    MinWeightKg = c.MinWeightKg,
                    MaxWeightKg = c.MaxWeightKg
                })
                .ToListAsync();
        }
        public async Task<CategoryDto> CreateAsync(SaveCategoryRequest request)
        {
            var categories = await _dbcontext.VehicleCategories.ToListAsync();

            ThrowIfInvalid(CategoryRequestValidator.Validate(request, categories));

            var category = new VehicleCategory
            {
                Name = request.Name.Trim(),
                IconKey = request.IconKey.Trim(),
                MinWeightKg = request.MinWeightKg
            };

            categories.Add(category);
            _dbcontext.VehicleCategories.Add(category);

            RecalculateUpperBounds(categories);
            ThrowIfInvalid(CategoryRangeValidator.Validate(categories));

            // One SaveChanges is one database transaction, so the whole change succeeds or fails together.
            await _dbcontext.SaveChangesAsync();
            return ToDto(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, SaveCategoryRequest request)
        {
            var categories = await _dbcontext.VehicleCategories.ToListAsync();

            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                throw new NotFoundException($"Category {id} was not found.");
            }

            var otherCategories = categories.Where(c => c.Id != id).ToList();
            ThrowIfInvalid(CategoryRequestValidator.Validate(request, otherCategories));

            category.Name = request.Name.Trim();
            category.IconKey = request.IconKey.Trim();
            category.MinWeightKg = request.MinWeightKg;

            RecalculateUpperBounds(categories);
            ThrowIfInvalid(CategoryRangeValidator.Validate(categories));

            await _dbcontext.SaveChangesAsync();
            return ToDto(category);
        }

        public async Task DeleteAsync(int id)
        {
            var categories = await _dbcontext.VehicleCategories.ToListAsync();

            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                throw new NotFoundException($"Category {id} was not found.");
            }

            if (categories.Count == 1)
            {
                throw new ValidationFailedException("The last remaining category cannot be deleted.");
            }

            var wasLowest = category.MinWeightKg == categories.Min(c => c.MinWeightKg);

            categories.Remove(category);
            _dbcontext.VehicleCategories.Remove(category);

            // If the lowest category is deleted, the next one takes over the bottom of the range.
            if (wasLowest)
            {
                var newLowest = categories.OrderBy(c => c.MinWeightKg).First();
                newLowest.MinWeightKg = 0;
            }

            // The category below absorbs the deleted range, because its end follows the next start.
            RecalculateUpperBounds(categories);
            ThrowIfInvalid(CategoryRangeValidator.Validate(categories));

            await _dbcontext.SaveChangesAsync();
        }

        // Each category ends where the next one starts. The highest category has no end.
        private static void RecalculateUpperBounds(List<VehicleCategory> categories)
        {
            var sorted = categories.OrderBy(c => c.MinWeightKg).ToList();

            for (var i = 0; i < sorted.Count; i++)
            {
                if (i == sorted.Count - 1)
                {
                    sorted[i].MaxWeightKg = null;
                }
                else
                {
                    sorted[i].MaxWeightKg = sorted[i + 1].MinWeightKg;
                }
            }
        }

        private static void ThrowIfInvalid(IEnumerable<string> errors)
        {
            var errorList = errors.ToList();
            if (errorList.Count > 0)
            {
                throw new ValidationFailedException(errorList);
            }
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
}
