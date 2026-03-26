using ErrorOr;
using Gridify;
using Company.ProjectName.Api.Persistence;

namespace Company.ProjectName.Api.Features.WeatherForecasts;

public sealed class WeatherForecastService(AppDbContext db) : IWeatherForecastService
{
    public ErrorOr<Paging<WeatherForecast>> GetAll(GridifyQuery query) =>
        db.WeatherForecasts.Gridify(query);

    public ErrorOr<WeatherForecast> GetById(int id)
    {
        var forecast = db.WeatherForecasts.Find(id);
        return forecast is null
            ? Error.NotFound(description: $"WeatherForecast with id {id} was not found.")
            : forecast;
    }
}
