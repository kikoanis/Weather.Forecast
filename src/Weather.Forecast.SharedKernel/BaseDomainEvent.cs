using MediatR;

namespace Weather.Forecast.SharedKernel;

/// <summary>
///   Represents a Domain Event.
/// </summary>
public abstract class BaseDomainEvent : INotification
{
  /// <summary>
  ///   Date and time when the event has occurred.
  /// </summary>
  public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
