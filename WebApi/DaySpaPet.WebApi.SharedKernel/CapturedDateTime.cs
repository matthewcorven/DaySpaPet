using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace DaySpaPet.WebApi.SharedKernel;

/// <summary>
/// Represents a captured Local date and time with time zone information and daylight saving time flag.
/// </summary>
/// <ref>https://nodatime.org/3.1.x/api/NodaTime.Instant.html</ref>
/// <ref>https://nodatime.org/3.1.x/api/NodaTime.LocalDateTime.html</ref>
/// <ref>https://www.youtube.com/watch?v=ZLJLfImuFqM</ref>
[ComplexType]
public record CapturedDateTime
{
  public Instant UtcInstant { get; set; } // SQL Server datetimeoffset, PostgreSQL "timestamp with time zone", where "with time zone" actually meants "in UTC"
  public bool IsDaylightSavingTime { get; set; } // SQL Server bit, PostgreSQL boolean
  [Required]
  public required string TimeZoneId { get; set; } // SQL Server nvarchar(100), PostgreSQL text
  public LocalDateTime LocalOriginDateTime { get; set; } // SQL Server datetime2, PostgreSQL "timestamp without time zone"

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

    var localNow = LocalDateTime.FromDateTime(localDateTime);
    return new CapturedDateTime
    {
      UtcInstant = Instant.FromDateTimeUtc(DateTime.UtcNow),
      IsDaylightSavingTime = isDst,
      TimeZoneId = timeZoneId,
      LocalOriginDateTime = localNow
    };
  }

  public static CapturedDateTime Empty => new()
  {
    UtcInstant = Instant.MinValue,
    IsDaylightSavingTime = false,
    TimeZoneId = string.Empty,
    LocalOriginDateTime = LocalDateTime.FromDateTime(DateTime.MinValue)
  };

  public static bool IsEmpty(CapturedDateTime capturedDateTime)
  {
    return capturedDateTime == Empty;
  }
}
