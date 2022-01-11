using Ardalis.ApiEndpoints;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Annotations;
using Weather.Forecast.Core.CityAggregate;
using Weather.Forecast.Core.CityAggregate.Specifications;
using Weather.Forecast.Core.Interfaces;
using Weather.Forecast.SharedKernel.Helpers;
using Weather.Forecast.SharedKernel.Interfaces;

namespace Weather.Forecast.Web.Endpoints.WeatherForecastEndPoints;

/// <summary>
///   An end point to get a city weather forecast from database by city WoeId<br />
///   See <see href="https://en.wikipedia.org/wiki/WOEID" />.
/// </summary>
public class GetByCityWoeId : BaseAsyncEndpoint
    .WithRequest<GetByCityWoeIdRequest>
    .WithResponse<GetByCityWoeIdResponse>
{
  private readonly IRepository<City> _repository;
  private readonly IMemoryCache _memoryCache;
  private readonly ICityService _cityService;

  public GetByCityWoeId(IRepository<City> repository, IMemoryCache memoryCache, ICityService cityService)
  {
    _repository = repository;
    _memoryCache = memoryCache;
    _cityService = cityService;
  }

  [HttpGet(GetByCityWoeIdRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets a single city weather forecast from database or from Meta Weather service https://www.metaweather.com",
      Description = "Gets a single city weather forecast by city woeId",
      OperationId = "City.GetByCityWoeId",
      Tags = new[] { "Weather Forecast Endpoints" })
  ]
  public override async Task<ActionResult<GetByCityWoeIdResponse>> HandleAsync([FromRoute] GetByCityWoeIdRequest request, CancellationToken cancellationToken = default)
  {
    var spec = new CityByWoeIdWithWeatherForecastSpec(request.WoeId);
    string key = $"{WeatherForecastCacheHelper.CacheKey}-{request.WoeId}";

    var city = await _memoryCache.GetOrCreate(key, async (entry) =>
    {
      entry.SetOptions(WeatherForecastCacheHelper.GetMemoryCacheEntryOptions());
      return await _repository.GetBySpecAsync(spec, cancellationToken);
    });

    if (city == null)
    {
      var result = await _cityService.SearchForCitiesByWoeId(request.WoeId, cancellationToken);
      switch (result.Status)
      {
        case ResultStatus.Ok:
        {
          city = result.Value;
          if (city != null)
          {
            _memoryCache.Remove($"{WeatherForecastCacheHelper.CacheKey}-all");
            _memoryCache.Remove(key);
              // only persist it if it was fetched throw service
              await _repository.AddAsync(city, cancellationToken);
          }

          break;
        }

        case ResultStatus.Invalid:
          return BadRequest(result.ValidationErrors);

        case ResultStatus.NotFound:
          return NotFound();
      }
    }

    if (city == null)
    {
      return NotFound();
    }

    var response = new GetByCityWoeIdResponse(city);
    return Ok(response);
  }
}
