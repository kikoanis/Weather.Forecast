using Microsoft.AspNetCore.Mvc;

namespace Weather.Forecast.Web.Controllers;

/// <summary>
///   Home controller. It will only be used to redirect to swagger.
///   The whole web app is only an API end points.
/// </summary>
public class HomeController : Controller
{
  public IActionResult Index()
  {
    return RedirectPermanent("/swagger");
  }
}
