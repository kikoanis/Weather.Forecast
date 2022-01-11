using Microsoft.Extensions.Caching.Memory;
using Weather.Forecast.Core.CityAggregate;
using Weather.Forecast.Core.CityAggregate.Specifications;
using Weather.Forecast.Core.Interfaces;
using Weather.Forecast.Scheduler.Scheduling;
using Weather.Forecast.SharedKernel.Helpers;
using Weather.Forecast.SharedKernel.Interfaces;

namespace Weather.Forecast.Scheduler;

public class FetchCityWeatherForecastTask : IScheduledTask
{
  private readonly IRepository<City> _repository;
  private readonly IWeatherForecastService _weatherForecastService;
  private readonly IMemoryCache _memoryCache;

  /// <summary>
  ///   Scheduled task to refresh stored cities weather forecasts if it was last retrieved 4 hours prior.
  ///   The task is scheduled to run every 10 minutes and will check for all cities that their weather forecast
  ///   have not been retrieved with 4 hours.
  /// </summary>
  /// <param name="repository">Injected City Repository.</param>
  /// <param name="weatherForecastService">Injected Weather Forecast Service.</param>
  /// <param name="memoryCache">Injected Memory Cache Service</param>
  public FetchCityWeatherForecastTask(IRepository<City> repository, IWeatherForecastService weatherForecastService, IMemoryCache memoryCache)
  {
    _repository = repository;
    _weatherForecastService = weatherForecastService;
    _memoryCache = memoryCache;
  }

  // Every 10 minutes
  public string Schedule => "*/10 * * * *";

  public const int NoOfHours = 4;

  /// <inheritdoc />
  public async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var spec = new AllCitiesWithWeatherForecastSpec();
      var allCities = await _repository.ListAsync(spec, cancellationToken);
      
      foreach (var city in allCities.Where(city => city.WeatherForecast?.LastFetchDateTime.AddHours(NoOfHours) < DateTime.UtcNow))
      {
        city.WeatherForecast = await _weatherForecastService.FetchWeatherForecast(city.WoeId, cancellationToken);
        if (city.WeatherForecast != null)
        {
          _memoryCache.Remove($"{WeatherForecastCacheHelper.CacheKey}-all");
          _memoryCache.Remove($"{WeatherForecastCacheHelper.CacheKey}-{city.Title}");
          _memoryCache.Remove($"{WeatherForecastCacheHelper.CacheKey}-{city.WoeId}");
          await _repository.UpdateAsync(city, cancellationToken);
        }
      }
    }
    catch (Exception e)
    {
      // if it fails, swallow exception, may be log it
      Console.WriteLine(e);
    }
  }
}
