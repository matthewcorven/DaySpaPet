using Bogus;
using DaySpaPet.WebApi.UseCases.Clients;
using DaySpaPet.WebApi.UseCases.Clients.ListShallow;
using System.Linq;

namespace DaySpaPet.WebApi.Infrastructure.Data.Queries;
public class FakeListClientsShallowQueryService
        : IListClientsShallowQueryService {
  public async Task<IEnumerable<ClientDTO>> ListAsync(int? take, int? skip) {
    int idIterator = 0;
    Faker<ClientDTO> faked = new Faker<ClientDTO>()
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

    IEnumerable<ClientDTO> result = faked.Generate(250).Skip(skip ?? 0).Take(take ?? 0);
    return await Task.FromResult(result).ConfigureAwait(false);
  }
}