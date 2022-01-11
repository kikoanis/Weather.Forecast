using Newtonsoft.Json;
using Weather.Forecast.SharedKernel;
using Weather.Forecast.SharedKernel.Interfaces;

namespace Weather.Forecast.Core.CityAggregate;

/// <summary>
///   Domain entity that represents City.
/// </summary>
public class City:BaseEntity, IAggregateRoot
{
  public WeatherForecast? WeatherForecast { get; set; }

  [JsonProperty("title")] public string Title { get; set; } = string.Empty;

  [JsonProperty("location_type")] public string LocationType { get; set; } = string.Empty;

  [JsonProperty("woeid")] public int WoeId { get; set; }

  [JsonProperty("latt_long")] public string LatLong { get; set; } = string.Empty;
}
