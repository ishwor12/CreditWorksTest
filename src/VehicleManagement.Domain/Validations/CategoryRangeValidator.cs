using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Domain.Validations
{
    /// <summary>
    /// Validates  categories maps every positive weight to exactly one category.
    /// Ranges are half-open [Min, Max).
    /// </summary>
    public static class CategoryRangeValidator
    {
        public static IReadOnlyList<string> Validate(IReadOnlyCollection<VehicleCategory> categories)
        {
            var errors = new List<string>();

            if (categories.Count == 0)
            {
                errors.Add("At least one category is required.");
                return errors;
            }

            var sorted = categories.OrderBy(c => c.MinWeightKg).ToList();

            foreach (var category in sorted)
            {
                if (HasMoreThanTwoDecimals(category.MinWeightKg) ||
                    (category.MaxWeightKg is { } boundary && HasMoreThanTwoDecimals(boundary)))
                {
                    errors.Add($"'{category.Name}': boundaries support minimum of two decimal places.");
                }

                if (category.MaxWeightKg is { } max && max <= category.MinWeightKg)
                {
                    errors.Add($"'{category.Name}':  maximum must be greater than the minimum.");
                }
            }

            var lowest = sorted[0];
            if (lowest.MinWeightKg != 0)
            {
                errors.Add($"The lowest category must start at 0 kg, but '{lowest.Name}' starts at {lowest.MinWeightKg} kg.");
            }

            // Logic:Adjacent categories must meet exactly: previous Max == next Min .
            for (var i = 0; i < sorted.Count - 1; i++)
            {
              var current = sorted[i];
            var next = sorted[i + 1];

                if (current.MaxWeightKg == null)
                {
                    errors.Add($"'{current.Name}' has no upper limit, so it overlaps '{next.Name}'.");
                }
                else if (current.MaxWeightKg < next.MinWeightKg)
                {
                    errors.Add($"There is a gap between {current.MaxWeightKg} kg and {next.MinWeightKg} kg ('{current.Name}' to '{next.Name}').");
                }
                else if (current.MaxWeightKg > next.MinWeightKg)
                {
                    errors.Add($"'{current.Name}' overlaps '{next.Name}' between {next.MinWeightKg} kg and {current.MaxWeightKg} kg.");
                }
            }

            var highest = sorted[sorted.Count - 1];
            if (highest.MaxWeightKg != null)
            {
                errors.Add($"The highest category '{highest.Name}' must have no upper limit.");
            }

            return errors;
        }
        /// <summary>
        /// Helper method: takes a number and answers true or false rounding based on 2 digits decimal place.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True or False</returns>

        private static bool HasMoreThanTwoDecimals(decimal value) => decimal.Round(value, 2) != value;


    }
}
