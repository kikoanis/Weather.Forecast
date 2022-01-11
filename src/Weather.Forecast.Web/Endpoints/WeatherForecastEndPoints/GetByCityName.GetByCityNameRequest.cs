
namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class GetByCityNameRequest
{
  public const string Route = "/city/{City}";
  public static string BuildRoute(string city) => Route.Replace("{City}", city);

  public string City { get; set; } = string.Empty;
}
