using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleManagement.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        // A separate database,for integration tests .
        private const string TestConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=CreditWorks_Tests;Trusted_Connection=True;TrustServerCertificate=True";

        public TestWebApplicationFactory()
        {
            // Environment variables override to appsettings.json and are read before Program.cs runs.
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", TestConnectionString);
        }
    }
}
