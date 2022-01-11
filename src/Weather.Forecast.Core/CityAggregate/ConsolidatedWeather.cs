using Newtonsoft.Json;
using Weather.Forecast.SharedKernel;

namespace Weather.Forecast.Core.CityAggregate;


/// <summary>
///   Domain entity that represent a consolidated weather.
/// </summary>
public class ConsolidatedWeather: BaseEntity
{
  public int WeatherForecastId { get; set; }

  [JsonProperty("id")] public long IdObject { get; set; }

  [JsonProperty("weather_state_name")] public string WeatherStateName { get; set; } = string.Empty;

  [JsonProperty("weather_state_abbr")] public string WeatherStateAbbr { get; set; } = string.Empty;

  [JsonProperty("applicable_date")] public DateTime ApplicableDate { get; set; }


  // Other props are commented out for brevity
  #region Other Props
  //[JsonProperty("wind_direction_compass")]
  //public string WindDirectionCompass { get; set; } = string.Empty;

  //[JsonProperty("created")] public DateTime Created { get; set; }

  //[JsonProperty("min_temp")] public double MinTemp { get; set; }

  //[JsonProperty("max_temp")] public double MaxTemp { get; set; }

  //[JsonProperty("the_temp")] public double TheTemp { get; set; }

  //[JsonProperty("wind_speed")] public double WindSpeed { get; set; }

  //[JsonProperty("wind_direction")] public double WindDirection { get; set; }

  //[JsonProperty("air_pressure")] public double AirPressure { get; set; }

  //[JsonProperty("humidity")] public int Humidity { get; set; }

  //[JsonProperty("visibility")] public double Visibility { get; set; }

  //[JsonProperty("predictability")] public int Predictability { get; set; }
  #endregion
}
