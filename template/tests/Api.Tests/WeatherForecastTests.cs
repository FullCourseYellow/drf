using System.Net;
using Company.ProjectName.Api.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        // Ensure schema is created for the in-memory database
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
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
