using Bogus;

namespace DaySpaPet.WebApi.Infrastructure.Data.BB.Extensions;

public static class FakerExtensions {
  public static T WeightedRandom<T>(this Faker f, params (T item, float weight)[] items) {
    float[] weights = items.Select(i => i.weight).ToArray();
    T[] choices = items.Select(i => i.item).ToArray();
    return f.Random.WeightedRandom(choices, weights);
  }
}