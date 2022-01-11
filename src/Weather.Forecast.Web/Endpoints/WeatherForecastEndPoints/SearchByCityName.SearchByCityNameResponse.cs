
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class SearchByCityNameResponse
{
  public SearchByCityNameResponse(IList<City> cities)
  {
    Cities = cities;
  }

  public IList<City> Cities { get; set; }
}
