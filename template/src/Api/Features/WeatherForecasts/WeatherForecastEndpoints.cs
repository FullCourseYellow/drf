using Company.ProjectName.Api.Extensions;
using Gridify;

namespace Company.ProjectName.Api.Features.WeatherForecasts;

public static class WeatherForecastEndpoints
{
    public static RouteGroupBuilder MapWeatherForecasts(this RouteGroupBuilder group)
    {
        var forecasts = group.MapGroup("/weather-forecasts")
            .WithTags("WeatherForecasts");

        forecasts.MapGet("/", async (
            IWeatherForecastService service,
            [AsParameters] GridifyQuery query) =>
            (await service.GetAllAsync(query)).MatchToResult())
            .WithName("GetWeatherForecasts")
            .WithSummary("List weather forecasts with filtering, sorting, and pagination");

        forecasts.MapGet("/{id:int}", async (
            IWeatherForecastService service,
            int id) =>
            (await service.GetByIdAsync(id)).MatchToResult())
            .WithName("GetWeatherForecastById")
            .WithSummary("Get a weather forecast by id");

        return group;
    }
}
