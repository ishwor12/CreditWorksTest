using System.Net;
using System.Net.Http.Json;
using VehicleManagement.Application.DTOs;
using Xunit;

namespace VehicleManagement.IntegrationTests;

public class CategoriesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CategoriesApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // The categories as they are seeded. Each test changes its own copy.
    private static List<CategoryInputDto> CreateSeedCategories()
    {
        return new List<CategoryInputDto>
           {
               new CategoryInputDto { Id = 1, Name = "Light", IconKey = "scooter", MinWeightKg = 0m, MaxWeightKg = 500m },
               new CategoryInputDto { Id = 2, Name = "Medium", IconKey = "car", MinWeightKg = 500m, MaxWeightKg = 2500m },
               new CategoryInputDto { Id = 3, Name = "Heavy", IconKey = "truck", MinWeightKg = 2500m, MaxWeightKg = null }
           };
    }

    private async Task<HttpResponseMessage> SaveAsync(List<CategoryInputDto> categories)
    {
        return await _client.PutAsJsonAsync("/api/categories", categories);
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
    public async Task Save_UnchangedCategories_Returns200()
    {
        await _factory.ResetDatabaseAsync();

        var response = await SaveAsync(CreateSeedCategories());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Save_WithAGap_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[1].MinWeightKg = 600m;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("gap", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_WithAnOverlap_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[0].MaxWeightKg = 600m;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("overlaps", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_MovingASharedBoundary_UpdatesBothCategories()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[1].MaxWeightKg = 2000m;
        categories[2].MinWeightKg = 2000m;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = await GetCategoriesAsync();
        Assert.Equal(2000m, saved[1].MaxWeightKg);
        Assert.Equal(2000m, saved[2].MinWeightKg);
    }

    [Fact]
    public async Task Save_WithANewCategory_CreatesIt()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[0].MaxWeightKg = 200m;
        categories.Add(new CategoryInputDto { Name = "Super Light", IconKey = "bicycle", MinWeightKg = 200m, MaxWeightKg = 500m });

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = await GetCategoriesAsync();
        Assert.Equal(
            new List<string> { "Light", "Super Light", "Medium", "Heavy" },
            saved.Select(c => c.Name).ToList());
    }

    [Fact]
    public async Task Save_WithACategoryLeftOut_DeletesIt()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories.RemoveAt(1);
        categories[0].MaxWeightKg = 2500m;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var saved = await GetCategoriesAsync();
        Assert.Equal(new List<string> { "Light", "Heavy" }, saved.Select(c => c.Name).ToList());
    }

    [Fact]
    public async Task Save_WhenHighestCategoryHasAnUpperLimit_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[2].MaxWeightKg = 5000m;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("must have no upper limit", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_WhenLowestCategoryDoesNotStartAtZero_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[0].MinWeightKg = 100m;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("start at 0", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_WithAnEmptyName_Returns400WithTheRowNumber()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[1].Name = "";

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Row 2", body);
        Assert.Contains("category name is required", body);
    }

    [Fact]
    public async Task Save_WithADuplicateName_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[1].Name = "light";

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("used more than once", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_WithAnEmptyList_Returns400()
    {
        await _factory.ResetDatabaseAsync();

        var response = await SaveAsync(new List<CategoryInputDto>());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("At least one category is required", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_WithAnUnknownId_Returns400()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[0].Id = 999;

        var response = await SaveAsync(categories);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("no longer exists", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Save_WhenInvalid_ChangesNothing()
    {
        await _factory.ResetDatabaseAsync();
        var categories = CreateSeedCategories();
        categories[1].MinWeightKg = 600m;

        var response = await SaveAsync(categories);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var saved = await GetCategoriesAsync();
        Assert.Equal(500m, saved[1].MinWeightKg);
    }
}