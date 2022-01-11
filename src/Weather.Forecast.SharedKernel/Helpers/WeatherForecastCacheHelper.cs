using Microsoft.Extensions.Caching.Memory;

namespace Weather.Forecast.SharedKernel.Helpers;

/// <summary>
///   A helper class used mainly for cache operations in different weather forecast end points.
/// </summary>
public static class WeatherForecastCacheHelper
{
  public const string CacheKey = "Cities";

  public const int CacheDuration = 60; // in minutes

  public static MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
  {
    return new MemoryCacheEntryOptions().SetAbsoluteExpiration(relative: TimeSpan.FromMinutes(CacheDuration));
  }
}


