using ErrorOr;
using Gridify;
using Company.ProjectName.Api.Persistence;

namespace Company.ProjectName.Api.Features.WeatherForecasts;

public sealed class WeatherForecastService(AppDbContext db) : IWeatherForecastService
{
    public Task<ErrorOr<Paging<WeatherForecast>>> GetAllAsync(GridifyQuery query)
    {
        try
        {
            ErrorOr<Paging<WeatherForecast>> result = db.WeatherForecasts.Gridify(query);
            return Task.FromResult(result);
        }
        catch (GridifyFilteringException ex)
        {
            return Task.FromResult<ErrorOr<Paging<WeatherForecast>>>(
                Error.Validation(description: $"Invalid filter expression: {ex.Message}"));
        }
    }

    public async Task<ErrorOr<WeatherForecast>> GetByIdAsync(int id)
    {
        var forecast = await db.WeatherForecasts.FindAsync(id);
        return forecast is null
            ? Error.NotFound(description: $"WeatherForecast with id {id} was not found.")
            : forecast;
    }
}
