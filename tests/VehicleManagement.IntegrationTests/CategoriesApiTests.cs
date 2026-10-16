using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using VehicleManagement.Application.DTOs;

namespace VehicleManagement.IntegrationTests
{
    public class CategoriesApiTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CategoriesApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _client.GetFromJsonAsync<List<CategoryDto>>("/api/categories");
            Assert.NotNull(categories);
            return categories;
        }

        [Fact]
        public async Task GetAll_ReturnsTheSeededCategoriesInWeightOrder()
        {
            await _factory.ResetDatabaseAsync();

            var categories = await GetCategoriesAsync();

            Assert.Equal(new List<string> { "Light", "Medium", "Heavy" }, categories.Select(c => c.Name).ToList());
            Assert.Null(categories[2].MaxWeightKg);
        }

        [Fact]
        public async Task Create_ValidCategory_Returns201AndSplitsTheRange()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "Super Light", IconKey = "bicycle", MinWeightKg = 200m };

            var response = await _client.PostAsJsonAsync("/api/categories", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var categories = await GetCategoriesAsync();
            Assert.Equal(
                new List<string> { "Light", "Super Light", "Medium", "Heavy" },
                categories.Select(c => c.Name).ToList());
            Assert.Equal(200m, categories[0].MaxWeightKg);
            Assert.Equal(500m, categories[1].MaxWeightKg);
        }

        [Fact]
        public async Task Create_DuplicateName_Returns400()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "Light", IconKey = "bicycle", MinWeightKg = 200m };

            var response = await _client.PostAsJsonAsync("/api/categories", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("already exists", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Create_MinimumUsedByAnotherCategory_Returns400()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "Another", IconKey = "bicycle", MinWeightKg = 500m };

            var response = await _client.PostAsJsonAsync("/api/categories", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("already starts at", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Create_EmptyNameAndIcon_Returns400WithBothMessages()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "", IconKey = "", MinWeightKg = 200m };

            var response = await _client.PostAsJsonAsync("/api/categories", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("Category name is required", body);
            Assert.Contains("Category icon is required", body);
        }

        [Fact]
        public async Task Update_MovingHeavyStart_ChangesWhereMediumEnds()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "Heavy", IconKey = "truck", MinWeightKg = 2000m };

            var response = await _client.PutAsJsonAsync("/api/categories/3", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var categories = await GetCategoriesAsync();
            Assert.Equal(2000m, categories[1].MaxWeightKg);
            Assert.Equal(2000m, categories[2].MinWeightKg);
        }

        [Fact]
        public async Task Update_LowestCategoryNotStartingAtZero_Returns400()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "Light", IconKey = "scooter", MinWeightKg = 100m };

            var response = await _client.PutAsJsonAsync("/api/categories/1", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("start at 0", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Update_UnknownId_Returns404()
        {
            await _factory.ResetDatabaseAsync();
            var request = new SaveCategoryRequest { Name = "Ghost", IconKey = "car", MinWeightKg = 100m };

            var response = await _client.PutAsJsonAsync("/api/categories/9999", request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_MiddleCategory_LetsTheLowerOneAbsorbItsRange()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.DeleteAsync("/api/categories/2");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            var categories = await GetCategoriesAsync();
            Assert.Equal(new List<string> { "Light", "Heavy" }, categories.Select(c => c.Name).ToList());
            Assert.Equal(2500m, categories[0].MaxWeightKg);
        }

        [Fact]
        public async Task Delete_LowestCategory_MakesTheNextOneStartAtZero()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.DeleteAsync("/api/categories/1");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            var categories = await GetCategoriesAsync();
            Assert.Equal(new List<string> { "Medium", "Heavy" }, categories.Select(c => c.Name).ToList());
            Assert.Equal(0m, categories[0].MinWeightKg);
        }

        [Fact]
        public async Task Delete_UnknownId_Returns404()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.DeleteAsync("/api/categories/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_LastRemainingCategory_Returns400()
        {
            await _factory.ResetDatabaseAsync();
            await _client.DeleteAsync("/api/categories/1");
            await _client.DeleteAsync("/api/categories/2");

            var response = await _client.DeleteAsync("/api/categories/3");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("last remaining", await response.Content.ReadAsStringAsync());
        }
    }
    }
