using Ardalis.SmartEnum;

namespace DaySpaPet.WebApi.Core.ClientAggregate;
public sealed class ClientAccountStatus : SmartEnum<ClientAccountStatus, string> {
    public static readonly ClientAccountStatus NotSet = new(nameof(NotSet), "!");
    public static readonly ClientAccountStatus New = new(nameof(New), "N");
    public static readonly ClientAccountStatus Active = new(nameof(Active), "A");
    public static readonly ClientAccountStatus Deactive = new(nameof(Deactive), "D");
    public static readonly ClientAccountStatus Closed = new(nameof(Closed), "C");

    public static Func<ClientAccountStatus> Default => () => NotSet;
    public static Func<ClientAccountStatus, bool> IsNotSet => _ => _ == NotSet;

    private ClientAccountStatus(string name, string value) : base(name, value) { }
}