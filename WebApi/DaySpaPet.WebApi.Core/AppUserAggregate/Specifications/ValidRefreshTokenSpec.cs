using Ardalis.Specification;

namespace DaySpaPet.WebApi.Core.AppUserAggregate.Specifications;

public class ValidRefreshTokenSpec : SingleResultSpecification<AppUserRefreshToken> {
  public ValidRefreshTokenSpec(Guid userId, string refreshToken) {
    Query.Where(x => 
      x.Id == userId 
      && x.RefreshToken == refreshToken 
      && x.RefreshExpiry.ToDateTimeUtc() >= DateTime.UtcNow);
  }
}