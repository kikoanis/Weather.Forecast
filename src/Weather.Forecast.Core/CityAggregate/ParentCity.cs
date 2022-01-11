using Newtonsoft.Json;
using Weather.Forecast.SharedKernel;

namespace Weather.Forecast.Core.CityAggregate;

/// <summary>
///   Domain entity that represent a parent city.
/// </summary>
public class ParentCity : BaseEntity
{
  [JsonProperty("title")] public string Title { get; set; } = string.Empty;

  [JsonProperty("location_type")] public string LocationType { get; set; } = string.Empty;

  [JsonProperty("woeid")] public int WoeId { get; set; }

  [JsonProperty("latt_long")] public string LatLong { get; set; } = string.Empty;
}
