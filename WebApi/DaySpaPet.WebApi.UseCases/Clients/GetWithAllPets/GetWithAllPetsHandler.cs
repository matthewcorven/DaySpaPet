using Ardalis.Result;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases.Pets;

namespace DaySpaPet.WebApi.UseCases.Clients.GetWithAllPets;

public sealed class GetWithAllPetsHandler
        : IQueryHandler<GetWithAllPetsQuery, Result<ClientWithPetsDTO>> {
  private readonly IRepository<Client> _repository;

  public GetWithAllPetsHandler(IRepository<Client> repository) {
    _repository = repository;
  }

  public async Task<Result<ClientWithPetsDTO>> Handle(GetWithAllPetsQuery request, CancellationToken cancellationToken) {
    Client? client = await _repository.GetByIdAsync(request.ClientId, cancellationToken);
    if (client == null) {
      return Result.NotFound();
    }

    ClientWithPetsDTO clientWithPets = new(client.Id, client.FirstName, client.LastName,
            client.PhoneCountryCode, client.PhoneNumber, client.PhoneExtension!, client.EmailAddress!,
            client.Pets.Select(p =>
                    new PetDTO(p.Id, p.ClientId, p.Name, p.Type, p.Breed,
                            p.Weight, p.Status, p.Age, p.BirthDate, p.AdoptionDate, p.DeathDate,
                            p.MostRecentVisitDate, p.FirstVisitDate, p.Notes.Select(pn =>
                                    new PetNoteDTO(pn.Id, pn.PetId, pn.Text, pn.IsAlert)).ToList())).ToList().AsReadOnly());
    return Result<ClientWithPetsDTO>.Success(clientWithPets);
  }
}