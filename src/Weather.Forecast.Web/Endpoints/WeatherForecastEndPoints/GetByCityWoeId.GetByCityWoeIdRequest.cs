
namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class GetByCityWoeIdRequest
{
  public const string Route = "/city/woeid/{WoeId:int}";
  public static string BuildRoute(int woeId) => Route.Replace("{WoeId:int}", woeId.ToString());

  public int WoeId { get; set; }
}
