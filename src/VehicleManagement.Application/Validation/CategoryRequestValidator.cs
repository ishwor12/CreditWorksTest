using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Application.Validation
{
    public static class CategoryRequestValidator
    {
        private const int MaxNameLength = 100;
        private const int MaxIconKeyLength = 50;

        // otherCategories is every category except which is being edited.
        public static List<string> Validate(SaveCategoryRequest request, List<VehicleCategory> otherCategories)
        {
            var errors = new List<string>();

            var name = request.Name?.Trim() ?? string.Empty;
            if (name.Length == 0)
            {
                errors.Add("Category name is required.");
            }
            else if (name.Length > MaxNameLength)
            {
                errors.Add($"Category name must be {MaxNameLength} characters or fewer.");
            }
            else if (otherCategories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"A category named '{name}' already exists.");
            }

            var iconKey = request.IconKey?.Trim() ?? string.Empty;
            if (iconKey.Length == 0)
            {
                errors.Add("Category icon is required.");
            }
            else if (iconKey.Length > MaxIconKeyLength)
            {
                errors.Add($"Category icon must be {MaxIconKeyLength} characters or fewer.");
            }

            if (request.MinWeightKg < 0)
            {
                errors.Add("Minimum weight cannot be negative.");
            }
            else if (decimal.Round(request.MinWeightKg, 2) != request.MinWeightKg)
            {
                errors.Add("Minimum weight supports at most two decimal places.");
            }
            else if (otherCategories.Any(c => c.MinWeightKg == request.MinWeightKg))
            {
                errors.Add($"Another category already starts at {request.MinWeightKg} kg.");
            }

            return errors;
        }
    }
}
