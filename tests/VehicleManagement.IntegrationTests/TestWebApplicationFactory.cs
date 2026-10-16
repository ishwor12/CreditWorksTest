using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VehicleManagement.Infrastructure.ApplicationDbContext;

namespace VehicleManagement.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        // A separate database,for integration tests .
        private const string TestConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=CreditWorks_Tests;Trusted_Connection=True;TrustServerCertificate=True";

        // Puts the database back to its seeded state: no vehicles, and the three original categories with their original IDs.
        private const string ResetSql = @"
   DELETE FROM Vehicles;
   DELETE FROM VehicleCategories;
   SET IDENTITY_INSERT VehicleCategories ON;
   INSERT INTO VehicleCategories (Id, Name, IconKey, MinWeightKg, MaxWeightKg) VALUES
       (1, 'Light', 'scooter', 0, 500),
       (2, 'Medium', 'car', 500, 2500),
       (3, 'Heavy', 'truck', 2500, NULL);
   SET IDENTITY_INSERT VehicleCategories OFF;";

        public TestWebApplicationFactory()
        {
            // Environment variables override to appsettings.json and are read before Program.cs runs.
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", TestConnectionString);
        }
        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.ExecuteSqlRawAsync(ResetSql);
        }
    }
}
