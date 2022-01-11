using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Weather.Forecast.Core.Interfaces;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

/// <summary>
///   An end point to get a city weather forecast from database by city name.<br />
///   It fall back to retrieve it from meta weather service if nothing is found in database.
/// </summary>
public class SearchByCityName : BaseAsyncEndpoint
    .WithRequest<SearchByCityNameRequest>
    .WithResponse<SearchByCityNameResponse>
{
  private readonly ICityService _cityService;

  public SearchByCityName(ICityService cityService)
  {
    _cityService = cityService;
  }

  [HttpGet(SearchByCityNameRequest.Route)]
  [SwaggerOperation(
    Summary = "Searches for a city or more from https://www.metaweather.com",
    Description =
      "Gets a list of cities that includes the provided string from data base or from meta weather..",
    OperationId = "City.SearchForCitiesByString",
    Tags = new[] {"Weather Forecast Endpoints"})
  ]
  public override async Task<ActionResult<SearchByCityNameResponse>> HandleAsync(
    [FromRoute] SearchByCityNameRequest request, CancellationToken cancellationToken = default)
  {
      var result  = await _cityService.SearchForCitiesByString(request.City, cancellationToken);
      switch (result.Status)
      {
        case ResultStatus.Invalid:
          return BadRequest(result.ValidationErrors);

        case ResultStatus.NotFound:
          return NotFound();
      }

    var response = new SearchByCityNameResponse(result.Value);
    return Ok(response);
  }
}
