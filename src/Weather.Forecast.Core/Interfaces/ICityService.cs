using Ardalis.Result;
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Core.Interfaces;

/// <summary>
///   An interface for city service.
/// </summary>
public interface ICityService
{
  /// <summary>
  ///   Fetches all cities that include a specified string from meta weather services.
  /// </summary>
  /// <param name="str">Searched string.</param>
  /// <param name="cancellationToken">Cancellation Token.</param>
  /// <returns>List of <see cref="City"/> if found, otherwise empty list.</returns>
  /// <remarks>
  /// The service will only filter out location type of "City"
  /// In case of more than one city found it will return the first 25 (again this could be done with paging better)
  /// In case of an exception it return all error messages as an error result.
  /// </remarks>
  Task<Result<IList<City>>> SearchForCitiesByString(string str, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Fetches one city by WoeId from meta weather services.
  /// </summary>
  /// <param name="woeId">WoeId.</param>
  /// <param name="cancellationToken">Cancellation Token.</param>
  /// <returns>A <see cref="City"/> if found, otherwise Not Found result.</returns>
  /// <remarks>
  /// In case of an exception it return all error messages as an error result.
  /// </remarks>
  Task<Result<City>> SearchForCitiesByWoeId(int woeId, CancellationToken cancellationToken = default);
}
