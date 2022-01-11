namespace Weather.Forecast.SharedKernel;

/// <summary>
///   Abstract class for all domain classes to implement.
///   It contains Id and a list of domain events.
/// </summary>
public abstract class BaseEntity
{
  public int Id { get; set; }

  // Not used in the entire application.
  // but for time constrains I was not able to implement domain event emitter and handler.
  public List<BaseDomainEvent> Events = new();
}
