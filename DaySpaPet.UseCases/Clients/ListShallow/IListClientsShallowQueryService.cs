namespace DaySpaPet.UseCases.Clients.ListShallow;

public interface IListClientsShallowQueryService
{
  Task<IEnumerable<ClientDTO>> ListAsync();
}
