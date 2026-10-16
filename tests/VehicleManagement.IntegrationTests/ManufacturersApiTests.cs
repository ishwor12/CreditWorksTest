using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.IntegrationTests
{
    public class ManufacturersApiTests:IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ManufacturersApiTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/manufacturers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsTheFiveSeededManufacturersSortedByName()
        {
            var manufacturers = await _client.GetFromJsonAsync<List<ManufacturerDto>>("/api/manufacturers");

            Assert.NotNull(manufacturers);
            var names = manufacturers.Select(m => m.Name).ToList();
            Assert.Equal(new List<string> { "Ferrari", "Honda", "Mazda", "Mercedes", "Toyota" }, names);
        }
    }
}
