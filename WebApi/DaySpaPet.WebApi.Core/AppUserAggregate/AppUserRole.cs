using Ardalis.GuardClauses;
using DaySpaPet.WebApi.SharedKernel;
using static System.Net.Mime.MediaTypeNames;
using EntityBase = DaySpaPet.WebApi.SharedKernel.EntityBase;

namespace DaySpaPet.WebApi.Core.AppUserAggregate;
public class AppUserRole : EntityBase<Guid> {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public AppUserRole()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF
  }

  public AppUserRole(string shortName, string longName) {
    ShortName = Guard.Against.NullOrEmpty(shortName, nameof(shortName));
    LongName = Guard.Against.NullOrEmpty(longName, nameof(longName));
  }

  public string ShortName { get; private set; }
  public string LongName { get; private set; }
}
