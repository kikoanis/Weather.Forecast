using Ardalis.Specification;

namespace Weather.Forecast.SharedKernel.Interfaces;

/// <summary>
///   An interface for read only repositories. Based on <seealso cref="IReadRepositoryBase{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
