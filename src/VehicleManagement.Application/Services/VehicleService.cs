using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.Abstractions;
using VehicleManagement.Application.DTOs;
using VehicleManagement.Application.Exceptions;
using VehicleManagement.Application.IServices;
using VehicleManagement.Application.Validation;
using VehicleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace VehicleManagement.Application.Services
{
    internal class VehicleService : IVehicleService
    {
        private readonly IAppDbContext _dbcontext;

        public VehicleService(IAppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
        {
            var errors = VehicleRequestValidator.Validate(request, DateTime.UtcNow.Year);
            if (errors.Count > 0)
            {
                throw new ValidationFailedException(errors);
            }

            var manufacturer = await _dbcontext.Manufacturers.FirstOrDefaultAsync(m => m.Id == request.ManufacturerId);
            if (manufacturer == null)
            {
                throw new ValidationFailedException("The selected manufacturer does not exist.");
            }

            var vehicle = new Vehicle
            {
                OwnerName = request.OwnerName.Trim(),
                ManufacturerId = manufacturer.Id,
                YearOfManufacture = request.YearOfManufacture,
                WeightKg = request.WeightKg
            };

            _dbcontext.Vehicles.Add(vehicle);
            await _dbcontext.SaveChangesAsync();

            var categories = await _dbcontext.VehicleCategories.ToListAsync();
            return ToDto(vehicle, manufacturer.Name, categories);
        }

        //  category are calculated from  weight,never stored on vehicle itself.
        private static VehicleDto ToDto(Vehicle vehicle, string manufacturerName, List<VehicleCategory> categories)
        {
            var category = categories.FirstOrDefault(c => c.Contains(vehicle.WeightKg));
            if (category == null)
            {
                throw new InvalidOperationException(
                    $"No category covers a weight of {vehicle.WeightKg} kg. The category configuration is invalid.");
            }

            return new VehicleDto
            {
                Id = vehicle.Id,
                OwnerName = vehicle.OwnerName,
                ManufacturerName = manufacturerName,
                YearOfManufacture = vehicle.YearOfManufacture,
                WeightKg = vehicle.WeightKg,
                CategoryName = category.Name,
                CategoryIconKey = category.IconKey
            };

        }
    }
}
