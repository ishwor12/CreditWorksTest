using VehicleManagement.Application.DTOs;

namespace VehicleManagement.Application.Validation;

public static class CategoryInputValidator
{
    private const int MaxNameLength = 100;
    private const int MaxIconKeyLength = 50;

    // Checks the name and icon of every row. The weight ranges are checked by CategoryRangeValidator.
    public static List<string> Validate(List<CategoryInputDto> inputs)
    {
        var errors = new List<string>();

        if (inputs.Count == 0)
        {
            errors.Add("At least one category is required.");
            return errors;
        }

        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var usedIds = new HashSet<int>();

        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            var row = i + 1;

            var name = (input.Name ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                errors.Add($"Row {row}: category name is required.");
            }
            else if (name.Length > MaxNameLength)
            {
                errors.Add($"Row {row}: category name must be {MaxNameLength} characters or fewer.");
            }
            else if (!usedNames.Add(name))
            {
                errors.Add($"The category name '{name}' is used more than once.");
            }

            var iconKey = (input.IconKey ?? string.Empty).Trim();
            if (iconKey.Length == 0)
            {
                errors.Add($"Row {row}: category icon is required.");
            }
            else if (iconKey.Length > MaxIconKeyLength)
            {
                errors.Add($"Row {row}: category icon must be {MaxIconKeyLength} characters or fewer.");
            }

            if (input.Id != null && !usedIds.Add(input.Id.Value))
            {
                errors.Add($"Row {row}: the same category appears more than once.");
            }
        }

        return errors;
    }
}