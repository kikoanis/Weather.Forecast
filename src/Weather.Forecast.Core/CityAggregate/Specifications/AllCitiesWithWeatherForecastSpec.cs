using Ardalis.Specification;

namespace Weather.Forecast.Core.CityAggregate.Specifications;

/// <summary>
///   All Cities with weather forecast specification.
/// </summary>
public sealed class AllCitiesWithWeatherForecastSpec : Specification<City>
{
  /// <summary>
  ///   Tries to retrieve all saved cities in database along with weather forecast entity and all consolidated weather (save 6 days) list if found.
  /// </summary>
  public AllCitiesWithWeatherForecastSpec()
  {
    Query
      .Include(city => city.WeatherForecast)
      .Include(city => city.WeatherForecast!.ConsolidatedWeatherList);

  }
}
