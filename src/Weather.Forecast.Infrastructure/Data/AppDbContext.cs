using Ardalis.EFCore.Extensions;
using Weather.Forecast.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weather.Forecast.Core.CityAggregate;

namespace Weather.Forecast.Infrastructure.Data;

public class AppDbContext : DbContext
{
  private readonly IMediator? _mediator;


  public AppDbContext(DbContextOptions<AppDbContext> options, IMediator? mediator)
      : base(options)
  {
    _mediator = mediator;
  }

  public DbSet<City> Cities => Set<City>();
  
  public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
  
  public DbSet<ConsolidatedWeather> ConsolidatedWeathers => Set<ConsolidatedWeather>();


  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_mediator == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
        .Select(e => e.Entity)
        .Where(e => e.Events.Any())
        .ToArray();

    foreach (var entity in entitiesWithEvents)
    {
      var events = entity.Events.ToArray();
      entity.Events.Clear();
      foreach (var domainEvent in events)
      {
        await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
      }
    }

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
