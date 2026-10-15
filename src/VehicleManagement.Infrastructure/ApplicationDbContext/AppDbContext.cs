using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Application.Abstractions;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Infrastructure.ApplicationDbContext
{
    public  class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options),IAppDbContext
    {
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();

        public DbSet<VehicleCategory> VehicleCategories => Set<VehicleCategory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    }
}
