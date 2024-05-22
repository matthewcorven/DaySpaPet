using Ardalis.SmartEnum;

namespace DaySpaPet.WebApi.Core;

public class AnimalType : SmartEnum<AnimalType, string>
{
	public static readonly AnimalType NotSet = new(nameof(NotSet), Facts.NotSet);
	public static readonly AnimalType Dog = new(nameof(Dog), "D");
	public static readonly AnimalType Cat = new(nameof(Cat), "C");

	public static Func<AnimalType> Default => () => NotSet;
	public static Func<AnimalType, bool> IsNotSet => _ => _ == NotSet;

	protected AnimalType(string name, string value) : base(name, value) { }
}
