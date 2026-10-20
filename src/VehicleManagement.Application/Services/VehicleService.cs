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
        public async Task<List<VehicleDto>> GetAllAsync(string? sortBy, bool descending)
        {
            var sortField = (sortBy ?? "owner").Trim().ToLowerInvariant();

            // AsNoTracking: we only read, so EF doesn't need to watch these rows for changes.
            IQueryable<Vehicle> query = _dbcontext.Vehicles.AsNoTracking().Include(v => v.Manufacturer);

            // Sorting runs in SQL. Each branch picks the column and the direction.
            IOrderedQueryable<Vehicle> ordered;

            if (sortField == "owner")
            {
                ordered = descending ? query.OrderByDescending(v => v.OwnerName) : query.OrderBy(v => v.OwnerName);
            }
            else if (sortField == "manufacturer")
            {
                // The "!" tells the compiler that Manufacturer is loaded by the Include above.
                ordered = descending ? query.OrderByDescending(v => v.Manufacturer!.Name) : query.OrderBy(v => v.Manufacturer!.Name);
            }
            else if (sortField == "year")
            {
                ordered = descending ? query.OrderByDescending(v => v.YearOfManufacture) : query.OrderBy(v => v.YearOfManufacture);
            }
            else if (sortField == "weight")
            {
                ordered = descending ? query.OrderByDescending(v => v.WeightKg) : query.OrderBy(v => v.WeightKg);
            }
            else
            {
                throw new ValidationFailedException("Sort by must be one of: owner, manufacturer, year, weight.");
            }

            // The Id tie-breaker keeps the order stable when two vehicles share the same value.
            var vehicles = await ordered.ThenBy(v => v.Id).ToListAsync();
            var categories = await _dbcontext.VehicleCategories.ToListAsync();

            return vehicles.Select(v => ToDto(v, v.Manufacturer!.Name, categories)).ToList();
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

            // fetch catrgories first
            var categories = await _dbcontext.VehicleCategories.ToListAsync();

            var vehicle = new Vehicle
            {
                OwnerName = request.OwnerName.Trim(),
                ManufacturerId = manufacturer.Id,
                YearOfManufacture = request.YearOfManufacture,
                WeightKg = request.WeightKg
            };

            // Checking for  category Mactheing before saving
            var category = categories.FirstOrDefault(c => c.Contains(vehicle.WeightKg));
            if (category == null)
            {
                throw new ValidationFailedException(
                    $"No category covers a weight of {vehicle.WeightKg} kg. The category configuration is invalid.");
            }

            _dbcontext.Vehicles.Add(vehicle);
            await _dbcontext.SaveChangesAsync();

            return ToDto(vehicle, manufacturer.Name, category);
        }

        // Update ToDto to take the specific category instead of  list
        private static VehicleDto ToDto(Vehicle vehicle, string manufacturerName, VehicleCategory category)
        {
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
