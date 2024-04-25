using DaySpaPet.WebApi.UseCases.Clients;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using Bogus;

namespace DaySpaPet.WebApi.Infrastructure.Data.Queries;
public class FakeListClientsShallowQueryService
  : IListClientsShallowQueryService
{
  public async Task<IEnumerable<ClientDTO>> ListAsync()
  {
    var idIterator = 0;
    var faked = new Faker<ClientDTO>()
      .UseSeed(1338)
      // Ensure all properties have rules. By default, StrictMode is false
      // Set a global policy by using Faker.DefaultStrictMode
      .StrictMode(true)
      // Id is deterministic
      .RuleFor(c => c.Id, f => idIterator++)
      .RuleFor(c => c.FirstName, f => f.Person.FirstName)
      .RuleFor(c => c.LastName, f => f.Person.LastName)
      .RuleFor(c => c.PhoneCountryCode, "+1")
      .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber("###-##-####"))
      .RuleFor(c => c.PhoneExtension, f => f.Random.Replace("###").OrNull(f, .92f))
      .RuleFor(c => c.EmailAddress, f => f.Person.Email);
    
    var result = faked.Generate(250);
    return await Task.FromResult(result).ConfigureAwait(false);
  }
}
