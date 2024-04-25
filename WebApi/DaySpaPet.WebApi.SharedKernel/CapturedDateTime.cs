using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace DaySpaPet.WebApi.SharedKernel;

/// <summary>
/// Represents a captured Local date and time with time zone information and daylight saving time flag.
/// </summary>
/// <ref>https://nodatime.org/3.1.x/api/NodaTime.Instant.html</ref>
/// <ref>https://nodatime.org/3.1.x/api/NodaTime.LocalDateTime.html</ref>
/// <ref>https://www.youtube.com/watch?v=ZLJLfImuFqM</ref>
public record CapturedDateTime
{
  public Instant Universal { get; set; } // SQL Server datetimeoffset, PostgreSQL "timestamp with time zone", where "with time zone" actually meants "in UTC"
  public Offset UtcOffset { get; set; } // SQL Server float, PostgreSQL double precision
  public bool IsDaylightSavingTime { get; set; } // SQL Server bit, PostgreSQL boolean
  [Required]
  public required string TimeZoneId { get; set; } // SQL Server nvarchar(100), PostgreSQL text
  public LocalDateTime Local { get; set; } // SQL Server datetime2, PostgreSQL "timestamp without time zone"

  public static CapturedDateTime CaptureFromLocalBclDateTime(DateTime localDateTime, bool isDst, string timeZoneId)
  {
    if (localDateTime.Kind != DateTimeKind.Local)
    {
      throw new ArgumentException("DateTime kind must be Local", nameof(localDateTime));
    }
    if (string.IsNullOrWhiteSpace(timeZoneId))
    {
      throw new ArgumentException("TimeZoneId must be provided", nameof(timeZoneId));
    }

    DateTimeZone? dateTimeZone = DateTimeZoneProviders.Tzdb[timeZoneId] 
      ?? throw new ArgumentException("Invalid TimeZoneId", nameof(timeZoneId));

    var utcNow = Instant.FromDateTimeUtc(DateTime.UtcNow);
    var localNow = LocalDateTime.FromDateTime(localDateTime);
    var offSetDateTime = new OffsetDateTime(localNow, dateTimeZone.GetUtcOffset(utcNow));

    return new CapturedDateTime
    {
      Universal = Instant.FromDateTimeUtc(DateTime.UtcNow),
      UtcOffset = offSetDateTime.Offset,
      IsDaylightSavingTime = isDst,
      TimeZoneId = timeZoneId,
      Local = localNow
    };
  }

  public static CapturedDateTime Empty => new()
  {
    Universal = Instant.MinValue,
    UtcOffset = Offset.Zero,
    IsDaylightSavingTime = false,
    TimeZoneId = string.Empty,
    Local = LocalDateTime.FromDateTime(DateTime.MinValue)
  };

  public static bool IsEmpty(CapturedDateTime capturedDateTime)
  {
    return capturedDateTime == Empty;
  }
}
