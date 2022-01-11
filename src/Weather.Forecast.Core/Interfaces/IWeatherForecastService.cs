using Ardalis.Result;
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Core.Interfaces;

/// <summary>
///   An interface for weather forecast.
/// </summary>
public interface IWeatherForecastService 
{
  /// <summary>
  ///   Fetches a weather forecast for specific city woeId from meta weather services.
  /// </summary>
  /// <param name="woeId">City's WoeId</param>
  /// <param name="cancellationToken">Cancellation Token</param>
  /// <returns><see cref="WeatherForecast"/> if found, otherwise NotFound result.</returns>
  /// <remarks>
  ///   In case of an exception it return all error messages as an error result.
  /// </remarks>
  Task<Result<WeatherForecast?>> FetchWeatherForecast(int woeId, CancellationToken cancellationToken = default);
}
