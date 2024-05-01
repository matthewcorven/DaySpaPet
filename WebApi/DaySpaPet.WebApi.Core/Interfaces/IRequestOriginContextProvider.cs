namespace DaySpaPet.WebApi.Core.Interfaces;
public interface IRequestOriginContextProvider
{

  public OriginClock GetOriginClock();
}
