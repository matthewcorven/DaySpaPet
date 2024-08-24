using Ardalis.GuardClauses;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace DaySpaPet.WebApi.SharedKernel;

/// <summary>
/// A base class for DDD Entities. Includes support for domain events dispatched post-persistence.
/// If you prefer GUID Ids, change it here.
/// If you need to support both GUID and int IDs, change to EntityBase&lt;TId&gt; and use TId as the type for Id.
/// </summary>
public abstract class EntityBase : HasDomainEventsBase {
  public int Id { get; set; }
  public Instant CreatedAtServerInstantUtc { get; internal set; } // SQL Server datetimeoffset, PostgreSQL "timestamp with time zone", where "with time zone" actually meants "in UTC"
  public bool CreatedAtDaylightSavingTime { get; internal set; } // SQL Server bit, PostgreSQL boolean
  [Required]
  public string CreatedAtTimeZoneId { get; internal set; } = default!; // SQL Server nvarchar(100), PostgreSQL text
  public LocalDateTime CreatedAtOriginLocalDateTime { get; internal set; } // SQL Server datetime2, PostgreSQL "timestamp without time zone"

  public Instant? ModifiedAtServerInstantUtc { get; internal set; } // SQL Server datetimeoffset, PostgreSQL "timestamp with time zone", where "with time zone" actually meants "in UTC"
  public bool? ModifiedAtDaylightSavingTime { get; internal set; } // SQL Server bit, PostgreSQL boolean
  public string? ModifiedAtTimeZoneId { get; internal set; } = default!; // SQL Server nvarchar(100), PostgreSQL text
  public LocalDateTime? ModifiedAtOriginLocalDateTime { get; internal set; } // SQL Server datetime2, PostgreSQL "timestamp without time zone"

  public bool IsNew() {
    return Id == default;
  }

  public void SetCreatedAt(OriginClock originClock) {
    CreatedAtServerInstantUtc = Instant.FromDateTimeUtc(DateTime.UtcNow);
    CreatedAtDaylightSavingTime = originClock.IsDaylightSavingsTime;
    // TODO: Validate timeZoneId is one of known static values
    CreatedAtTimeZoneId = Guard.Against.NullOrEmpty(originClock.TimeZoneId, nameof(originClock));
    CreatedAtOriginLocalDateTime = originClock.LocalDateTime;
  }

  public void SetModifiedAt(OriginClock originClock) {
    ModifiedAtServerInstantUtc = Instant.FromDateTimeUtc(DateTime.UtcNow);
    ModifiedAtDaylightSavingTime = originClock.IsDaylightSavingsTime;
    // TODO: Validate timeZoneId is one of known static values
    ModifiedAtTimeZoneId = Guard.Against.NullOrEmpty(originClock.TimeZoneId, nameof(originClock));
    ModifiedAtOriginLocalDateTime = originClock.LocalDateTime;
  }
}

public abstract class EntityBase<TId> : HasDomainEventsBase
      where TId : struct, IEquatable<TId> {
  public TId Id { get; set; }
  public Instant CreatedAtServerInstantUtc { get; internal set; } // SQL Server datetimeoffset, PostgreSQL "timestamp with time zone", where "with time zone" actually meants "in UTC"
  public bool CreatedAtDaylightSavingTime { get; internal set; } // SQL Server bit, PostgreSQL boolean
  [Required]
  public string CreatedAtTimeZoneId { get; internal set; } = default!; // SQL Server nvarchar(100), PostgreSQL text
  public LocalDateTime CreatedAtOriginLocalDateTime { get; internal set; } // SQL Server datetime2, PostgreSQL "timestamp without time zone"

  public Instant? ModifiedAtServerInstantUtc { get; internal set; }
  public bool? ModifiedAtDaylightSavingTime { get; internal set; }
  public string? ModifiedAtTimeZoneId { get; internal set; } = default!;
  public LocalDateTime? ModifiedAtOriginLocalDateTime { get; internal set; }

  public bool IsNew() {
    return Id.Equals(default);
  }

  public void SetCreatedAt(OriginClock originClock) {
    CreatedAtServerInstantUtc = Instant.FromDateTimeUtc(DateTime.UtcNow);
    CreatedAtDaylightSavingTime = originClock.IsDaylightSavingsTime;
    // TODO: Validate timeZoneId is one of known static values
    CreatedAtTimeZoneId = Guard.Against.NullOrEmpty(originClock.TimeZoneId, nameof(originClock));
    CreatedAtOriginLocalDateTime = originClock.LocalDateTime;
  }
  public void SetModifiedAt(OriginClock originClock) {
    ModifiedAtServerInstantUtc = Instant.FromDateTimeUtc(DateTime.UtcNow);
    ModifiedAtDaylightSavingTime = originClock.IsDaylightSavingsTime;
    // TODO: Validate timeZoneId is one of known static values
    ModifiedAtTimeZoneId = Guard.Against.NullOrEmpty(originClock.TimeZoneId, nameof(originClock));
    ModifiedAtOriginLocalDateTime = originClock.LocalDateTime;
  }
}