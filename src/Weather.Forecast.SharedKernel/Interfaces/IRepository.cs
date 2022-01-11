using Ardalis.Specification;

namespace Weather.Forecast.SharedKernel.Interfaces;

/// <summary>
///   An interface for repositories. Based on <seealso cref="IRepositoryBase{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
