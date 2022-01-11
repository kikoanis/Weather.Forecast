namespace Weather.Forecast.Scheduler.Scheduling;

/// <summary>
/// An Interface for scheduled tasks.
/// </summary>
public interface IScheduledTask
{
  /// <summary>
  ///   Represents CRON string. 
  ///   see <see href="https://en.wikipedia.org/wiki/Cron"/> for more information.
  /// </summary>
  string Schedule { get; }

  /// <summary>
  ///   Executes a scheduled task.
  /// </summary>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>A task that represents an asynchronous operation.</returns>
  Task ExecuteAsync(CancellationToken cancellationToken);
}
