using Company.ProjectName.Api.Extensions;
using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace Company.ProjectName.Api.Features.WeatherForecasts;

public static class WeatherForecastEndpoints
{
    public static RouteGroupBuilder MapWeatherForecasts(this RouteGroupBuilder group)
    {
        var forecasts = group.MapGroup("/weather-forecasts")
            .WithTags("WeatherForecasts");

        forecasts.MapGet("/", (
            IWeatherForecastService service,
            [AsParameters] GridifyQuery query) =>
            service.GetAll(query).MatchToResult())
            .WithName("GetWeatherForecasts")
            .WithSummary("List weather forecasts with filtering, sorting, and pagination");

        forecasts.MapGet("/{id:int}", (
            IWeatherForecastService service,
            int id) =>
            service.GetById(id).MatchToResult())
            .WithName("GetWeatherForecastById")
            .WithSummary("Get a weather forecast by id");

        return group;
    }
}
