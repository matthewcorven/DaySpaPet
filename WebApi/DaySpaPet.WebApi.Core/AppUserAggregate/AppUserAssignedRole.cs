using Ardalis.GuardClauses;
using DaySpaPet.WebApi.SharedKernel;
using EntityBase = DaySpaPet.WebApi.SharedKernel.EntityBase;

namespace DaySpaPet.WebApi.Core.AppUserAggregate;
public class AppUserAssignedRole : EntityBase<Guid> {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public AppUserAssignedRole()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
    // Required for EF
  }

  public AppUserAssignedRole(Guid appUserId, Guid appUserRoleId, OriginClock originClock) {
    AppUserId = Guard.Against.NullOrEmpty(appUserId, nameof(appUserId));
    AppUserRoleId = Guard.Against.NullOrEmpty(appUserRoleId, nameof(appUserRoleId));

    this.SetCreatedAt(originClock);
  }

  public Guid AppUserId { get; private set; } // Required foreign key property
  public AppUser AppUser { get; private set; } = null!; // Required reference navigation to principal (ref: https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many)
  public Guid AppUserRoleId { get; private set; }
  public AppUserRole AppUserRole { get; private set; } = null!;
}
