using System.Net;
using System.Net.Http.Json;
using VehicleManagement.Application.DTOs;
using Xunit;

namespace VehicleManagement.IntegrationTests;

public class VehiclesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public VehiclesApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task CreateVehicleAsync(string owner, int manufacturerId, int year, decimal weightKg)
    {
        var request = new CreateVehicleRequest
        {
            OwnerName = owner,
            ManufacturerId = manufacturerId,
            YearOfManufacture = year,
            WeightKg = weightKg
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // Manufacturer IDs: 1 Mazda, 2 Mercedes, 3 Honda, 4 Ferrari.
    private async Task CreateFourVehiclesAsync()
    {
        await CreateVehicleAsync("John Smith", 1, 2019, 450m);
        await CreateVehicleAsync("Jane Turei", 2, 2015, 1200m);
        await CreateVehicleAsync("Aroha Ngata", 3, 2021, 3000m);
        await CreateVehicleAsync("Ben Carter", 4, 2010, 1850.75m);
    }

    private async Task<List<VehicleDto>> GetVehiclesAsync(string query = "")
    {
        var vehicles = await _client.GetFromJsonAsync<List<VehicleDto>>("/api/vehicles" + query);
        Assert.NotNull(vehicles);
        return vehicles;
    }

    [Fact]
    public async Task Create_ValidVehicle_Returns201WithItsCategory()
    {
        await _factory.ResetDatabaseAsync();
        var request = new CreateVehicleRequest
        {
            OwnerName = "John Smith",
            ManufacturerId = 1,
            YearOfManufacture = 2019,
            WeightKg = 450m
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>();
        Assert.NotNull(vehicle);
        Assert.Equal("Mazda", vehicle.ManufacturerName);
        Assert.Equal("Light", vehicle.CategoryName);
        Assert.Equal("scooter", vehicle.CategoryIconKey);
    }

    [Theory]
    [InlineData(1, "Light")]
    [InlineData(500, "Medium")]
    [InlineData(2499, "Medium")]
    [InlineData(2500, "Heavy")]
    public async Task Create_Weight_IsPlacedInTheCorrectCategory(int weightKg, string expectedCategory)
    {
        await _factory.ResetDatabaseAsync();
        var request = new CreateVehicleRequest
        {
            OwnerName = "John Smith",
            ManufacturerId = 1,
            YearOfManufacture = 2019,
            WeightKg = weightKg
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>();
        Assert.NotNull(vehicle);
        Assert.Equal(expectedCategory, vehicle.CategoryName);
    }

    [Fact]
    public async Task Create_InvalidRequest_Returns400WithAllMessages()
    {
        await _factory.ResetDatabaseAsync();
        var request = new CreateVehicleRequest
        {
            OwnerName = "",
            ManufacturerId = 0,
            YearOfManufacture = 1800,
            WeightKg = -5m
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("name is required", body);
        Assert.Contains("Manufacturer is required", body);
        Assert.Contains("Year of manufacture must be between", body);
        Assert.Contains("Weight must be greater than 0", body);
    }

    [Fact]
    public async Task Create_UnknownManufacturer_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var request = new CreateVehicleRequest
        {
            OwnerName = "John Smith",
            ManufacturerId = 99,
            YearOfManufacture = 2019,
            WeightKg = 450m
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("does not exist", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Create_WeightWithThreeDecimals_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var request = new CreateVehicleRequest
        {
            OwnerName = "John Smith",
            ManufacturerId = 1,
            YearOfManufacture = 2019,
            WeightKg = 10.123m
        };

        var response = await _client.PostAsJsonAsync("/api/vehicles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("two decimal places", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAll_WithNoVehicles_ReturnsAnEmptyList()
    {
        await _factory.ResetDatabaseAsync();

        var vehicles = await GetVehiclesAsync();

        Assert.Empty(vehicles);
    }

    [Fact]
    public async Task GetAll_DefaultSort_IsOwnerAToZ_WithCategories()
    {
        await _factory.ResetDatabaseAsync();
        await CreateFourVehiclesAsync();

        var vehicles = await GetVehiclesAsync();

        Assert.Equal(
            new List<string> { "Aroha Ngata", "Ben Carter", "Jane Turei", "John Smith" },
            vehicles.Select(v => v.OwnerName).ToList());
        Assert.Equal(
            new List<string> { "Heavy", "Medium", "Medium", "Light" },
            vehicles.Select(v => v.CategoryName).ToList());
    }

    [Fact]
    public async Task GetAll_SortByWeightDescending()
    {
        await _factory.ResetDatabaseAsync();
        await CreateFourVehiclesAsync();

        var vehicles = await GetVehiclesAsync("?sortBy=weight&descending=true");

        Assert.Equal(
            new List<decimal> { 3000m, 1850.75m, 1200m, 450m },
            vehicles.Select(v => v.WeightKg).ToList());
    }

    [Fact]
    public async Task GetAll_SortByYear()
    {
        await _factory.ResetDatabaseAsync();
        await CreateFourVehiclesAsync();

        var vehicles = await GetVehiclesAsync("?sortBy=year");

        Assert.Equal(
            new List<int> { 2010, 2015, 2019, 2021 },
            vehicles.Select(v => v.YearOfManufacture).ToList());
    }

    [Fact]
    public async Task GetAll_SortByManufacturer()
    {
        await _factory.ResetDatabaseAsync();
        await CreateFourVehiclesAsync();

        var vehicles = await GetVehiclesAsync("?sortBy=manufacturer");

        Assert.Equal(
            new List<string> { "Ferrari", "Honda", "Mazda", "Mercedes" },
            vehicles.Select(v => v.ManufacturerName).ToList());
    }

    [Fact]
    public async Task GetAll_UnknownSortField_Returns400()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/vehicles?sortBy=banana");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Sort by must be one of", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ChangingCategoryBoundary_ReclassifiesExistingVehicles()
    {
        await _factory.ResetDatabaseAsync();
        await CreateVehicleAsync("Jane Turei", 2, 2015, 2200m);

        var before = await GetVehiclesAsync();
        Assert.Equal("Medium", before[0].CategoryName);

        // Medium now ends at 2000 kg and Heavy starts at 2000 kg. This is the example from the assignment.
        var categories = new List<CategoryInputDto>
        {
            new CategoryInputDto { Id = 1, Name = "Light", IconKey = "scooter", MinWeightKg = 0m, MaxWeightKg = 500m },
            new CategoryInputDto { Id = 2, Name = "Medium", IconKey = "car", MinWeightKg = 500m, MaxWeightKg = 2000m },
            new CategoryInputDto { Id = 3, Name = "Heavy", IconKey = "truck", MinWeightKg = 2000m, MaxWeightKg = null }
        };
        var response = await _client.PutAsJsonAsync("/api/categories", categories);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var after = await GetVehiclesAsync();
        Assert.Equal("Heavy", after[0].CategoryName);
    }
}