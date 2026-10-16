using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.Application.Validation
{
    public class VehicleRequestValidator
    {
        private const int FirstCarYear = 1886;
        private const int MaxOwnerNameLength = 200;
        private const decimal MaxWeightKg = 1000000m;

        public static List<string> Validate(CreateVehicleRequest request, int currentYear)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.OwnerName))
            {
                errors.Add("Owner's name is required.");
            }
            else if (request.OwnerName.Trim().Length > MaxOwnerNameLength)
            {
                errors.Add($"Owner's name must be {MaxOwnerNameLength} characters or fewer.");
            }

            if (request.ManufacturerId <= 0)
            {
                errors.Add("Manufacturer is required.");
            }

            // Allowing next year's models, which can be sold before  year starts.
            var latestYear = currentYear + 1;
            if (request.YearOfManufacture < FirstCarYear || request.YearOfManufacture > latestYear)
            {
                errors.Add($"Year of manufacture must be between {FirstCarYear} and {latestYear}.");
            }

            if (request.WeightKg <= 0)
            {
                errors.Add("Weight must be greater than 0 kg.");
            }
            else if (request.WeightKg > MaxWeightKg)
            {
                errors.Add($"Weight must not exceed {MaxWeightKg} kg.");
            }
            else if (decimal.Round(request.WeightKg, 2) != request.WeightKg)
            {
                errors.Add("Weight supports at most two decimal places.");
            }

            return errors;
        }
    }
}
