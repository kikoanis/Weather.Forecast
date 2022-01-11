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
///   An end point to get all cities data saved in database.
/// </summary>
public class GetAllCities : BaseAsyncEndpoint
    .WithoutRequest
    .WithResponse<GetAllCitiesResponse>
{
  private readonly IRepository<City> _repository;
  private readonly IMemoryCache _memoryCache;

  public GetAllCities(IRepository<City> repository, IMemoryCache memoryCache)
  {
    _repository = repository;
    _memoryCache = memoryCache;
  }

  [HttpGet(GetAllCitiesRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets all saved cities in database",
      Description = "Gets all cities list saved in database ordered by city name (title)",
      OperationId = "City.GetAllCities",
      Tags = new[] { "Weather Forecast Endpoints" })
  ]
  public override async Task<ActionResult<GetAllCitiesResponse>> HandleAsync(CancellationToken cancellationToken = default)
  {
    var spec = new AllCitiesWithWeatherForecastSpec();
    const string key = $"{WeatherForecastCacheHelper.CacheKey}-all";

    var cityList = await _memoryCache.GetOrCreate(key, async (entry) =>
    {
      entry.SetOptions(WeatherForecastCacheHelper.GetMemoryCacheEntryOptions());
      return await _repository.ListAsync(spec, cancellationToken);
    });

    var response = new GetAllCitiesResponse(cityList);
    return Ok(response);
  }
}
