using Newtonsoft.Json;
using Weather.Forecast.SharedKernel;

namespace Weather.Forecast.Core.CityAggregate;

/// <summary>
///   Domain entity that represent a weather forecast.
/// </summary>
public class WeatherForecast : BaseEntity
{
  [JsonProperty("consolidated_weather")]
  public IEnumerable<ConsolidatedWeather> ConsolidatedWeatherList { get; set; } = new List<ConsolidatedWeather>();

  public IEnumerable<ConsolidatedWeather> OrderedConsolidatedWeatherList
  {
    get
    {
      return ConsolidatedWeatherList.OrderBy(item => item.ApplicableDate);
    }
  }

  public int CityId { get; set; }

  [JsonProperty("time")] public DateTime CreateDateTime { get; set; }

  public DateTime LastFetchDateTime { get; set; }

  // Other props are commented out for brevity

  #region Other props
  //[JsonProperty("sun_rise")] public DateTime SunRise { get; set; }

  //[JsonProperty("sun_set")] public DateTime SunSet { get; set; }

  //[JsonProperty("timezone_name")] public string TimezoneName { get; set; } = string.Empty;

  //[JsonProperty("parent")] public ParentCity ParentCity { get; set; }

  //[JsonProperty("sources")] public IList<WeatherForecastSource> Sources { get; set; }

  //[JsonProperty("title")] public string Title { get; set; } = string.Empty;

  //[JsonProperty("location_type")] public string LocationType { get; set; } = string.Empty;

  //[JsonProperty("woeid")] public int WoeId { get; set; }

  //[JsonProperty("latt_long")] public string LatLong { get; set; } = string.Empty;

  //[JsonProperty("timezone")] public string Timezone { get; set; } = string.Empty;
  #endregion

}
