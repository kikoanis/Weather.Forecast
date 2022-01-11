using Ardalis.Specification;

namespace Weather.Forecast.Core.CityAggregate.Specifications;

/// <summary>
///   City by name with weather forecast specification.
/// </summary>
public sealed class CityByNameWithWeatherForecastSpec : Specification<City>, ISingleResultSpecification
{
  /// <summary>
  ///   Tries to retrieve a city along with weather forecast entity and all consolidated weather (save 6 days) list if found.
  /// </summary>
  /// <param name="name">City name (case insensitive).</param>
  public CityByNameWithWeatherForecastSpec(string? name)
  {
    Query
      .Where(city => city.Title.ToLower() == name!.ToLower())
      .Include(city => city.WeatherForecast)
      .Include(city => city.WeatherForecast!.ConsolidatedWeatherList);

  }
}
