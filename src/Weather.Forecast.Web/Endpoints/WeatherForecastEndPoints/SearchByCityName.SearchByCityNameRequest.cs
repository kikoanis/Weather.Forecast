
namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

public class SearchByCityNameRequest
{
  public const string Route = @"/city/search/{City}";
  public static string BuildRoute(string city) => Route.Replace("{City}", city);

  public string City { get; set; } = string.Empty;
}
