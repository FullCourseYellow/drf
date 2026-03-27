using System.Net;
using Company.ProjectName.Api.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
#if (includeAuth)
using Microsoft.Extensions.Configuration;
#endif
using Xunit;

namespace Company.ProjectName.Api.Tests;

public sealed class WeatherForecastTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly SqliteConnection _connection;
    private readonly HttpClient _client;

    public WeatherForecastTests(WebApplicationFactory<Program> factory)
    {
        // Use a named in-memory SQLite connection shared across the test session
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
#if (includeAuth)
            // Supply dummy JWT Bearer config so the middleware registers during tests
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:JwtBearer:Authority"] = "https://test.example.com",
                    ["Authentication:JwtBearer:Audience"]  = "test-api",
                });
            });
#endif
            builder.ConfigureServices(services =>
            {
                // Replace the real DbContext options with in-memory SQLite
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor is not null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(_connection));
            });
        });

        _client = _factory.CreateClient();

        // Apply migrations to the in-memory database (mirrors production startup)
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }

    [Fact]
    public async Task GetWeatherForecasts_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/weather-forecasts?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"count\"", body);
        Assert.Contains("\"data\"", body);
    }

    [Fact]
    public async Task GetWeatherForecastById_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/v1/weather-forecasts/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        _connection.Dispose();
    }
}
