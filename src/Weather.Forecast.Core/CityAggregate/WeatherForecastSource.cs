using Newtonsoft.Json;
using Weather.Forecast.SharedKernel;

namespace Weather.Forecast.Core.CityAggregate;

/// <summary>
///   Domain entity that represent a weather forecast source.
/// </summary>
public class WeatherForecastSource : BaseEntity
{
  [JsonProperty("title")] public string Title { get; set; } = string.Empty;
  [JsonProperty("slug")] public string Slug { get; set; } = string.Empty;
  [JsonProperty("url")] public string Url { get; set; } = string.Empty;
  [JsonProperty("crawl_rate")] public int CrawlRate { get; set; }
}
