﻿using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using NodaTime;

namespace DaySpaPet.WebApi.SharedKernel;

/// <summary>
/// A base class for DDD Entities. Includes support for domain events dispatched post-persistence.
/// If you prefer GUID Ids, change it here.
/// If you need to support both GUID and int IDs, change to EntityBase&lt;TId&gt; and use TId as the type for Id.
/// </summary>
public abstract class EntityRootBase : HasDomainEventsBase
{
  public int Id { get; set; }
  public Instant CreatedAtServerInstantUtc { get; internal set; } // SQL Server datetimeoffset, PostgreSQL "timestamp with time zone", where "with time zone" actually meants "in UTC"
  public bool CreatedAtDaylightSavingTime { get; internal set; } // SQL Server bit, PostgreSQL boolean
  [Required]
  public string CreatedAtTimeZoneId { get; internal set; } = default!; // SQL Server nvarchar(100), PostgreSQL text
  public LocalDateTime CreatedAtOriginLocalDateTime { get; internal set; } // SQL Server datetime2, PostgreSQL "timestamp without time zone"
  
  public void SetCreatedAt(bool isDst, string timeZoneId, LocalDateTime originLocalDateTime)
  {
    CreatedAtServerInstantUtc = Instant.FromDateTimeUtc(DateTime.UtcNow);
    CreatedAtDaylightSavingTime = isDst;
    // TODO: Validate timeZoneId is one of known static values
    CreatedAtTimeZoneId = Guard.Against.NullOrEmpty(timeZoneId, nameof(timeZoneId));
    CreatedAtOriginLocalDateTime = originLocalDateTime;
  }
}

public abstract class EntityRootBase<TId> : HasDomainEventsBase
  where TId : struct, IEquatable<TId>
{
  public TId Id { get; set; }
}

public static class EntityRootBaseExtensions
{
  public static bool IsNew(this EntityRootBase entity)
  {
    return entity.Id == default;
  }

  public static bool IsNew<TId>(this EntityRootBase<TId> entity)
    where TId : struct, IEquatable<TId>
  {
    return entity.Id.Equals(default);
  }

  public static void SetCreatedAt(this EntityRootBase entity, bool isDst, string timeZoneId, LocalDateTime originLocalDateTime)
  {
    entity.CreatedAtServerInstantUtc = Instant.FromDateTimeUtc(DateTime.UtcNow);
    entity.CreatedAtDaylightSavingTime = isDst;
    entity.CreatedAtTimeZoneId = Guard.Against.NullOrEmpty(timeZoneId, nameof(timeZoneId));
    entity.CreatedAtOriginLocalDateTime = originLocalDateTime;
  }
}
