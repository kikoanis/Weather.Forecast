
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class GetByCityNameResponse
{
  public GetByCityNameResponse(City city)
  {
    City = city;
  }

  public City City { get; set; }
}
