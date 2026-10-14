using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Infrastructure.Configuration
{
    internal class VehicleCategoryConfiguration : IEntityTypeConfiguration<VehicleCategory>
    {
        public void Configure(EntityTypeBuilder<VehicleCategory> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.IconKey).IsRequired().HasMaxLength(50);
            builder.Property(c => c.MinWeightKg).HasPrecision(18, 2);
            builder.Property(c => c.MaxWeightKg).HasPrecision(18, 2);

            builder.HasIndex(c => c.Name).IsUnique();

            // CategoryRangeValidator
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_VehicleCategory_Range",
                "[MinWeightKg] >= 0 AND ([MaxWeightKg] IS NULL OR [MaxWeightKg] > [MinWeightKg])"));

            builder.HasData(
                new VehicleCategory { Id = 1, Name = "Light", IconKey = "scooter", MinWeightKg = 0m, MaxWeightKg = 500m },
                new VehicleCategory { Id = 2, Name = "Medium", IconKey = "car", MinWeightKg = 500m, MaxWeightKg = 2500m },
                new VehicleCategory { Id = 3, Name = "Heavy", IconKey = "truck", MinWeightKg = 2500m, MaxWeightKg = null });
        }
    
    }
}
