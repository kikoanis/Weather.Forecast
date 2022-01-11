using Ardalis.Result;
using Newtonsoft.Json;
using Weather.Forecast.Core.CityAggregate;
using Weather.Forecast.Core.CityAggregate.Constants;
using Weather.Forecast.Core.Interfaces;

namespace Weather.Forecast.Core.Services;

/// <summary>
///   Fetches a city from meta weather service.
/// </summary>
public class CityService : ICityService
{
  private readonly IWeatherForecastService _weatherForecastService;

  public CityService(IWeatherForecastService weatherForecastService)
  {
    _weatherForecastService = weatherForecastService;
  }

  ///<inheritdoc />
  public async Task<Result<IList<City>>> SearchForCitiesByString(string cityName, CancellationToken cancellationToken = default)
  {
    try
    {
      using var httpClient = new HttpClient();
      using var clientResponse =
        await httpClient.GetAsync(
          $"{WeatherForeCastServiceConstants.MetaWeatherUrl}/location/search/?query={cityName}",
          cancellationToken);
      string cityResponse = await clientResponse.Content.ReadAsStringAsync(cancellationToken);
      var cities = JsonConvert.DeserializeObject<List<City>>(cityResponse);


      return new Result<IList<City>>((cities ?? new List<City>()).Where(city => city.LocationType == "City").Take(25).ToList());
    }
    catch (Exception ex)
    {
      // Should be logged somewhere
      return Result<IList<City>>.Error(ex.Message);
    }
  }

  ///<inheritdoc />
  public async Task<Result<City>> SearchForCitiesByWoeId(int woeId, CancellationToken cancellationToken = default)
  {
    try
    {
      using var httpClient = new HttpClient();
      using var clientResponse =
        await httpClient.GetAsync(
          $"{WeatherForeCastServiceConstants.MetaWeatherUrl}/location/{woeId}",
          cancellationToken);
      string cityResponse = await clientResponse.Content.ReadAsStringAsync(cancellationToken);
      var city = JsonConvert.DeserializeObject<City>(cityResponse);

      if (city == null)
      {
        return Result<City>.NotFound();
      }

      city.WeatherForecast = await _weatherForecastService.FetchWeatherForecast(city.WoeId, cancellationToken);

      return new Result<City>(city);
    }
    catch (Exception ex)
    {
      // Should be logged somewhere
      return Result<City>.Error(ex.Message);
    }
  }
}
