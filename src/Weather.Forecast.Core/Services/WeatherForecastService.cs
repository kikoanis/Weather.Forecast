using Ardalis.Result;
using Newtonsoft.Json;
using Weather.Forecast.Core.CityAggregate;
using Weather.Forecast.Core.CityAggregate.Constants;
using Weather.Forecast.Core.Interfaces;

namespace Weather.Forecast.Core.Services;

/// <summary>
///   Fetches a weather forecast from meta weather service.
/// </summary>
public class WeatherForecastService : IWeatherForecastService
{
  /// <inheritdoc />
  public async Task<Result<WeatherForecast?>> FetchWeatherForecast(int woeId,
    CancellationToken cancellationToken = default)
  {
    try
    {
      using var httpClient = new HttpClient();

      using var clientForecastResponse =
        await httpClient.GetAsync($"{WeatherForeCastServiceConstants.MetaWeatherUrl}/location/{woeId}",
          cancellationToken);

      var foreCastResponse = await clientForecastResponse.Content.ReadAsStringAsync(cancellationToken);
      var weatherForecast = JsonConvert.DeserializeObject<WeatherForecast>(foreCastResponse);

      if (weatherForecast != null)
      {
        weatherForecast.LastFetchDateTime = DateTime.UtcNow;
      }

      return weatherForecast == null ? Result<WeatherForecast?>.NotFound() : new Result<WeatherForecast?>(weatherForecast);
    }
    catch (Exception ex)
    {
      // Should be logged somewhere
      return Result<WeatherForecast?>.Error(ex.Message);
    }
  }
}
