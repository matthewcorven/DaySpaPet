using Bogus;
using DaySpaPet.WebApi.Core;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.PetAggregate;
using DaySpaPet.WebApi.Infrastructure.Data.BB.Extensions;
using DaySpaPet.WebApi.SharedKernel;
using NodaTime;
using System.Globalization;

namespace DaySpaPet.WebApi.Infrastructure.Data.BB;

/// <summary>
/// Generates data for the database, in a deterministic fashion from a source of random data.
/// To be used for local development and automated testing, where each environment can expect
/// to have the same data and make assertions against.
/// 
/// Our seed data is generated using the Bogus library, which is a simple and sane fake data generator for .NET.
/// 
/// We are constructing data for a fictitious pet spa "BubbleBuddies" ("BB") that has been in business since 2022-01-01.
/// BB is open Monday through Saturday, with the earliest dropoff appointment at 7:00 AM and the latest at 11:00 AM and 
/// latest pickup is 5:00 PM. The Spa operations in Chattanooga, TN which is in Eastern time zone (America/New_York)
/// although situated very near the Central time zone so a minority of our Clients are from there.
/// </summary>
public sealed class BubbleBuddiesDatabaseSeeder {
  private static readonly DateTime _spaOpenedDateTime = DateTime.Parse("2022-01-01T08:20:34-05:00", new CultureInfo("en-US"));
  private static readonly Instant _spaOpenedInstant = Instant.FromDateTimeOffset(_spaOpenedDateTime);
  private static readonly DateTimeZone _spaTimeZone = DateTimeZoneProviders.Tzdb["America/New_York"];

  private readonly List<Client> _clients = [];
  public IReadOnlyCollection<Client> Clients => _clients.AsReadOnly();
  private readonly List<Pet> _pets = [];
  public IReadOnlyCollection<Pet> Pets => _pets.AsReadOnly();

  public BubbleBuddiesDatabaseSeeder() {
    //Set the randomizer seed if you wish to generate repeatable data sets.
    Randomizer.Seed = new Random(34845);
    // Cause runtime exceptions if, for any require property, a corresponding RuleFor has not been provided.
    Faker.DefaultStrictMode = true;

    _clients.AddRange(GenerateClients(amount: 250));

    int[] items = [1, 2, 3, 4, 5];
    float[] weights = new[] { 0.6f, 0.2f, 0.15f, 0.03f, 0.02f };

    _pets.AddRange(GeneratePetsForClients(items, weights));
  }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
  private static IReadOnlyCollection<Client> GenerateClients(int amount)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
  {
    int idIterator = 1;
    Faker<Client> clientFaker = new Faker<Client>()
            .RuleFor(c => c.Id, f => idIterator++)
            .RuleFor(c => c.FirstName, f => f.Person.FirstName)
            .RuleFor(c => c.LastName, f => f.Person.LastName)
            .RuleFor(c => c.PhoneCountryCode, "+1")
            .RuleFor(c => c.PhoneNumber, f => f.WeightedRandom(
                    ("423-###-####", 0.74f),
                    ("615-###-####", 0.08f),
                    ("310-###-####", 0.03f),
                    (f.Phone.PhoneNumber(), 0.15f) // Default fallback
                ))
            .RuleFor(c => c.PhoneExtension, f => f.Random.Replace("###").OrNull(f, .92f))
            .RuleFor(c => c.Status, f => f.WeightedRandom(
                    (ClientAccountStatus.Active, 0.45f),
                    (ClientAccountStatus.Deactive, 0.12f),
                    (ClientAccountStatus.Closed, 0.03f),
                    (ClientAccountStatus.New, 0.40f)
                ))
            .RuleFor(c => c.CreatedAtServerInstantUtc, f => GenerateCreatedAtServerInstantUtc(f))
            .RuleFor(c => c.CreatedAtDaylightSavingTime, (f, c) => GenerateCreatedAtDaylightSavingTime(c.CreatedAtServerInstantUtc))
            .RuleFor(c => c.CreatedAtTimeZoneId, (f, c) => GenerateCreatedAtTimeZoneId(c.CreatedAtServerInstantUtc))
            .RuleFor(c => c.CreatedAtOriginLocalDateTime, (f, c) => GenerateCreatedAtOriginLocalDateTime(c.CreatedAtServerInstantUtc))
            .RuleFor(c => c.EmailAddress, f => f.Person.Email.OrNull(f, 0.33f))
            .RuleFor(c => c.ModifiedAtServerInstantUtc, f => f.Random.Bool() ? Instant.FromDateTimeUtc(f.Date.Between(_spaOpenedDateTime, DateTime.UtcNow)) : null)
            .RuleFor(c => c.ModifiedAtTimeZoneId, (f, c) => c.ModifiedAtServerInstantUtc.HasValue ? _spaTimeZone.Id : null)
            .RuleFor(c => c.ModifiedAtDaylightSavingTime, (f, c) => c.ModifiedAtServerInstantUtc.HasValue && c.ModifiedAtServerInstantUtc.Value.InZone(_spaTimeZone).IsDaylightSavingTime())
            .RuleFor(c => c.ModifiedAtOriginLocalDateTime, (f, c) => c.ModifiedAtServerInstantUtc.HasValue ? c.ModifiedAtServerInstantUtc.Value.InZone(_spaTimeZone).LocalDateTime : null);

    System.Collections.ObjectModel.ReadOnlyCollection<Client> clients = Enumerable.Range(1, amount)
            .Select(i => SeedRow(clientFaker, i))
            .ToList()
            .AsReadOnly();

    return clients;
  }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
  private IReadOnlyCollection<Pet> GeneratePetsForClients(int[] items, float[] weights)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
  {
    List<Pet> pets = [];
    Randomizer random = new();

    Faker pf = new("en");
    DateTime oldestBirthDate = pf.Date.Past(20);
    DateTime youngestBirthDate = _spaOpenedDateTime.AddDays(-1);

    PetStatus[] petStatuses = [PetStatus.New, PetStatus.Active, PetStatus.Inactive];
    float[] puppyPetStatusWeights = new[] { 0.7f, 0.28f, 0.02f };
    float[] adultStatusWeights = new[] { 0.1f, 0.8f, 0.1f };
    float[] seniorStatusWeights = new[] { 0.05f, 0.55f, 0.4f };

    int petIdIterator = 1;
    foreach (Client client in Clients) {
      int countOfClientPets = random.WeightedRandom(items, weights);
      for (int p = 0; p < countOfClientPets - 1; p++) {
        DateOnly birthDate = DateOnly.FromDateTime(pf.Date.Between(oldestBirthDate, youngestBirthDate));
        int age = (int)Math.Floor((int)(_spaOpenedDateTime - birthDate.ToDateTime(TimeOnly.MinValue)).TotalDays / 365.25m);
        // Set created fields relative to birth date (e.g. usually 2+ months after between range of 1 day after birth and 1.5 years after birth).
        bool createdAtDst = true;
        string createdAtTimeZoneId = "America/New_York";
        LocalDateTime local = LocalDateTime.FromDateTime(birthDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).AddDays(pf.Random.Int(1, 547)));

        OriginClock originClock = new(local, createdAtTimeZoneId, createdAtDst);

        PetStatus status = age <= 1
                ? random.WeightedRandom(petStatuses, puppyPetStatusWeights)
                : age > 8
                        ? random.WeightedRandom(petStatuses, adultStatusWeights)
                        : random.WeightedRandom(petStatuses, seniorStatusWeights);
        double? weight = age <= 0
                ? pf.Random.Double(1, 10)
                : pf.Random.Double(10.01, 100);
        DateOnly? adoptionDate = pf.Random.Bool(0.2f)
                ? DateOnly.FromDateTime(birthDate.ToDateTime(TimeOnly.MinValue).AddYears(1))
                : null;
        DateOnly? deathDate = age <= 3
                ? pf.Random.Bool(0.6f)
                        ? DateOnly.FromDateTime(birthDate.ToDateTime(TimeOnly.MinValue).AddYears(9))
                        : null
                : null;
        DateOnly? firstVisitDate = status == PetStatus.New
                ? null
                : DateOnly.FromDateTime(birthDate.ToDateTime(TimeOnly.MinValue).AddMonths(pf.Random.Int(1, age * 12)));
        DateOnly? mostRecentVisitDate = firstVisitDate is null
                ? null
                : DateOnly.FromDateTime(((DateOnly)firstVisitDate).ToDateTime(TimeOnly.MinValue).AddDays(pf.Random.Int(1, 60)));

        OptionalNewPetData data = new(weight, age, birthDate, adoptionDate, deathDate, firstVisitDate, mostRecentVisitDate);
        Pet pet = new(client.Id, pf.Person.FirstName, AnimalType.Dog,
                GetRandomBreed(pf), originClock, data) {
          Id = petIdIterator++
        };
        pets.Add(pet);
      }
    }

    return pets.AsReadOnly();
  }

  private static string GetRandomBreed(Faker faker) {
    // TODO: Use a list of breeds: https://github.com/paiv/fci-breeds/blob/main/fci-breeds.csv
    string[] breeds = new[] { "Labrador Retriever", "German Shepherd", "Golden Retriever", "Bulldog", "Poodle", "Beagle", "Rottweiler", "Yorkshire Terrier", "Boxer", "Dachshund", "Siberian Husky", "Great Dane", "Doberman Pinscher", "Australian Shepherd", "Cavalier King Charles Spaniel", "Shih Tzu", "Boston Terrier", "Pembroke Welsh Corgi", "Maltese", "Chihuahua", "Pomeranian", "Shetland Sheepdog", "Miniature Schnauzer", "French Bulldog", "Cocker Spaniel", "Pug", "Weimaraner", "Border Collie", "Basset Hound", "Vizsla", "West Highland White Terrier", "Rhodesian Ridgeback", "Bernese Mountain Dog", "Havanese", "Newfoundland", "Bichon Frise", "Bloodhound", "Akita", "Papillon", "Belgian Malinois", "Alaskan Malamute", "Whippet", "Chinese Shar-Pei", "Samoyed", "Scottish Terrier", "Soft Coated Wheaten Terrier", "Portuguese Water Dog", "Bullmastiff", "Airedale Terrier", "Bull Terrier", "Irish Setter", "Cairn Terrier", "Italian Greyhound", "Collie", "Dalmatian", "German Shorthaired Pointer", "English Springer Spaniel", "Chow Chow", "Pekingese", "Lhasa Apso", "Borzoi", "Afghan Hound", "Old English Sheepdog", "Basenji", "Keeshond", "American Eskimo Dog", "Saluki", "Standard Schnauzer", "Irish Wolfhound", "Brussels Griffon", "Japanese Chin", "Norwegian Elkhound", "Silky Terrier", "Toy Fox Terrier", "Tibetan Terrier", "Wirehaired Pointing Griffon", "Black and Tan Coonhound", "Giant Schnauzer", "Gordon Setter", "English Toy Spaniel", "Norwich Terrier", "Pharaoh Hound", "Ibizan Hound" };
    return faker.PickRandom(breeds);
  }

  private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class {
    T recordRow = faker.UseSeed(rowId).Generate();
    return recordRow;
  }

  private static Instant GenerateCreatedAtServerInstantUtc(Faker f) {
    // Calculate the random time based on the 80% of clients within first 18 months.
    var maxMonths = (int)((DateTime.UtcNow - _spaOpenedDateTime).TotalDays / 30);
    var monthsSinceStart = f.Random.Int(0, (int)(0.8 * maxMonths));
    var dateTime = _spaOpenedDateTime.AddMonths(monthsSinceStart);

    // Adjusting for time of day with a 68% chance of morning time (before 11 AM dropoff).
    var hourOfDay = f.Random.Bool(0.68f) ? f.Random.Int(7, 11) : f.Random.Int(12, 17);
    var localDateTime = new LocalDateTime(dateTime.Year, dateTime.Month, dateTime.Day, hourOfDay, f.Random.Int(0, 59), f.Random.Int(0, 59));
    return localDateTime.InZoneLeniently(_spaTimeZone).ToInstant();
  }

  private static bool GenerateCreatedAtDaylightSavingTime(Instant createdAtServerInstantUtc) {
    var zonedDateTime = createdAtServerInstantUtc.InZone(_spaTimeZone);
    return zonedDateTime.IsDaylightSavingTime();
  }

  private static string GenerateCreatedAtTimeZoneId(Instant createdAtServerInstantUtc) {
    var zonedDateTime = createdAtServerInstantUtc.InZone(_spaTimeZone);
    return zonedDateTime.Zone.Id;
  }

  private static LocalDateTime GenerateCreatedAtOriginLocalDateTime(Instant createdAtServerInstantUtc) {
    var zonedDateTime = createdAtServerInstantUtc.InZone(_spaTimeZone);
    return zonedDateTime.LocalDateTime;
  }
}