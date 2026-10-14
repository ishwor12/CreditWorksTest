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

            var ordered = categories.OrderBy(c => c.MinWeightKg).ToList();

            foreach (var category in ordered)
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

            if (ordered[0].MinWeightKg != 0m)
            {
                errors.Add($"The lowest category must start at 0 kg, but '{ordered[0].Name}' started at {ordered[0].MinWeightKg} kg.");
            }

            // Logic:Adjacent categories must meet exactly: previous Max == next Min .
            for (var i = 0; i < ordered.Count - 1; i++)
            {
                var (current, next) = (ordered[i], ordered[i + 1]);

                if (current.MaxWeightKg is not { } upper)
                {
                    errors.Add($"'{current.Name}' has no upper limit, so it overlaps '{next.Name}'.");
                }
                else if (upper < next.MinWeightKg)
                {
                    errors.Add($"There is a gap between {upper} kg and {next.MinWeightKg} kg ('{current.Name}' to '{next.Name}').");
                }
                else if (upper > next.MinWeightKg)
                {
                    errors.Add($"'{current.Name}' overlaps '{next.Name}' between {next.MinWeightKg} kg and {upper} kg.");
                }
            }

            if (ordered[^1].MaxWeightKg is not null)
            {
                errors.Add($"The highest category '{ordered[^1].Name}' must have no upper limit.");
            }

            return errors;
        }
        /// <summary>
        /// Helper method: takes a number and answers true or false rounding based on 2 digits decimal place.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        private static bool HasMoreThanTwoDecimals(decimal value) => decimal.Round(value, 2) != value;


    }
}
