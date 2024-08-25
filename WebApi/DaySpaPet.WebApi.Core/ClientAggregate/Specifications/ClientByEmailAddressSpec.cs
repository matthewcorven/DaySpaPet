using Ardalis.Specification;

namespace DaySpaPet.WebApi.Core.ClientAggregate.Specifications;

public class ClientByEmailAddressSpec : SingleResultSpecification<Client> {
  public ClientByEmailAddressSpec(string emailAddress) {
    Query.Where(x => x.EmailAddress != null && x.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));
  }
}