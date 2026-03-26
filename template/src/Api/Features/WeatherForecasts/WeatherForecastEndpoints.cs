using Company.ProjectName.Api.Extensions;
using Company.ProjectName.Api.Persistence;
using Gridify;
using Microsoft.AspNetCore.Mvc;

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
            .WithSummary("List weather forecasts with filtering, sorting, and pagination")
            .Produces<Paging<WeatherForecast>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        forecasts.MapGet("/{id:int}", async (
            IWeatherForecastService service,
            int id) =>
            (await service.GetByIdAsync(id)).MatchToResult())
            .WithName("GetWeatherForecastById")
            .WithSummary("Get a weather forecast by id")
            .Produces<WeatherForecast>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }
}
