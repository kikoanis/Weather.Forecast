using Ardalis.Specification;

namespace Weather.Forecast.Core.CityAggregate.Specifications;

/// <summary>
///   City by WoeId with weather forecast specification.
///   Check <see href="https://en.wikipedia.org/wiki/WOEID" />.
///   for what is WoeId.
/// </summary>
public sealed class CityByWoeIdWithWeatherForecastSpec : Specification<City>, ISingleResultSpecification
{
  /// <summary>
  ///   Tries to retrieve a city along with weather forecast entity and all consolidated weather (save 6 days) list if found.
  /// </summary>
  /// <param name="woeId">City WoeId (a number that represent Where On Earth Identifier).</param>
  public CityByWoeIdWithWeatherForecastSpec(int woeId)
  {
    Query
      .Where(city => city.WoeId == woeId)
      .Include(city => city.WeatherForecast)
      .Include(city => city.WeatherForecast!.ConsolidatedWeatherList);
  }
}
