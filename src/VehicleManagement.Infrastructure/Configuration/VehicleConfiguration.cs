using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Infrastructure.Configuration
{
    internal class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.Property(v => v.OwnerName).IsRequired().HasMaxLength(200);
            builder.Property(v => v.WeightKg).HasPrecision(18, 2);

            // RestrictDeleteBehaviour: a manufacturer that is in use cannot be deleted.
            builder.HasOne(v => v.Manufacturer)
                .WithMany()
                .HasForeignKey(v => v.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // for upper bound  year changing over time.
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Vehicle_WeightKg_Positive", "[WeightKg] > 0");
                t.HasCheckConstraint("CK_Vehicle_Year_Minimum", "[YearOfManufacture] >= 1886");
            });
        }
    }
}
