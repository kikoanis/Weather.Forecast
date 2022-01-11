
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class GetAllCitiesResponse
{
  public GetAllCitiesResponse(IList<City> cities)
  {
    CityList = cities;
  }

  public IList<City> CityList { get; set; }
}
