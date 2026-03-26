using ErrorOr;
using Gridify;
using Company.ProjectName.Api.Persistence;

namespace Company.ProjectName.Api.Features.WeatherForecasts;

public interface IWeatherForecastService
{
    Task<ErrorOr<Paging<WeatherForecast>>> GetAllAsync(GridifyQuery query);
    Task<ErrorOr<WeatherForecast>> GetByIdAsync(int id);
}
