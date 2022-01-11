using Ardalis.Specification.EntityFrameworkCore;
using Weather.Forecast.SharedKernel.Interfaces;

namespace Weather.Forecast.Infrastructure.Data;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  public EfRepository(AppDbContext dbContext) : base(dbContext)
  {
  }
}
