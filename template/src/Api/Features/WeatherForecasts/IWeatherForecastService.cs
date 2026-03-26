using ErrorOr;
using Gridify;
using Company.ProjectName.Api.Persistence;

namespace Company.ProjectName.Api.Features.WeatherForecasts;

public interface IWeatherForecastService
{
    ErrorOr<Paging<WeatherForecast>> GetAll(GridifyQuery query);
    ErrorOr<WeatherForecast> GetById(int id);
}
