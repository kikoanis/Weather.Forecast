using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Weather.Forecast.Scheduler.Scheduling;

/// <summary>
///   Scheduler extensions.
/// </summary>
public static class SchedulerExtensions
{
  public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs>? unobservedTaskExceptionHandler)
  {
    return services.AddSingleton<IHostedService, SchedulerHostedService>(serviceProvider =>
    {
      var instance = new SchedulerHostedService(serviceProvider.GetServices<IScheduledTask>());
      instance.UnobservedTaskException += unobservedTaskExceptionHandler;
      return instance;
    });
  }
}
