using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Application.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<Vehicle> Vehicles { get; }

        DbSet<Manufacturer> Manufacturers { get; }

        DbSet<VehicleCategory> VehicleCategories { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
