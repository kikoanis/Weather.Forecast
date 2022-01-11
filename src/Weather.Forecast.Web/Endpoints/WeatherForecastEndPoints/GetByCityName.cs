using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using Weather.Forecast.Core.CityAggregate;
using Weather.Forecast.Core.CityAggregate.Specifications;
using Weather.Forecast.SharedKernel.Helpers;
using Weather.Forecast.SharedKernel.Interfaces;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

/// <summary>
///   An end point to get a city weather forecast from database by city name.
/// </summary>
public class GetByCityName : BaseAsyncEndpoint
    .WithRequest<GetByCityNameRequest>
    .WithResponse<GetByCityNameResponse>
{
  private readonly IRepository<City> _repository;
  private readonly IMemoryCache _memoryCache;

  /// <summary>
  ///   An end point to get a city weather forecast from database by city name.<br />
  /// </summary>
  public GetByCityName(IRepository<City> repository, IMemoryCache memoryCache)
  {
    _repository = repository;
    _memoryCache = memoryCache;
  }

  [HttpGet(GetByCityNameRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets a single city weather forecast from database",
      Description = "Gets a single city weather forecast by city name. City name must be exact case insensitive.",
      OperationId = "City.GetByCityName",
      Tags = new[] { "Weather Forecast Endpoints" })
  ]
  public override async Task<ActionResult<GetByCityNameResponse>> HandleAsync([FromRoute] GetByCityNameRequest request, CancellationToken cancellationToken = default)
  {
    var spec = new CityByNameWithWeatherForecastSpec(request.City);
    string key = $"{WeatherForecastCacheHelper.CacheKey}-{request.City}";

    var city = await _memoryCache.GetOrCreate(key, async (entry) =>
    {
      entry.SetOptions(WeatherForecastCacheHelper.GetMemoryCacheEntryOptions());
      return await _repository.GetBySpecAsync(spec, cancellationToken);
    });

    if (city == null)
    {
      return NotFound();
    }

    var response = new GetByCityNameResponse(city);
    return Ok(response);
  }
}
