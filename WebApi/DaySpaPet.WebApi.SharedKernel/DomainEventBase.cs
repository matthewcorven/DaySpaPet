using MediatR;
using NodaTime;

namespace DaySpaPet.WebApi.SharedKernel;

/// <summary>
/// A base type for domain events. Depends on MediatR INotification.
/// Includes DateOccurred which is set on creation.
/// </summary>
public record DomainEventBase : INotification {
  public Instant OccurredAtInstant { get; init; }
  public OriginClock OriginClock { get; init; }

  public DomainEventBase(OriginClock originClock) {
    OccurredAtInstant = Instant.FromDateTimeUtc(DateTime.UtcNow);
    OriginClock = originClock;
  }
}