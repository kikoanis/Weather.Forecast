
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class GetByCityWoeIdResponse
{
  public GetByCityWoeIdResponse(City city)
  {
    City = city;
  }

  public City City { get; set; }
}
