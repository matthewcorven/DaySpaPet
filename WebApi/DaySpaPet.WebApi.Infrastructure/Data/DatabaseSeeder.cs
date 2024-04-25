using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Intrinsics.X86;
using Bogus;
using DaySpaPet.WebApi.Core;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.PetAggregate;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Infrastructure.Data;
public sealed class DatabaseSeeder
{
  private static readonly DateTime _dateReference = DateTime.Parse("2022-01-01T00:00:00Z", new CultureInfo("en-US"));

  private readonly List<Client> _clients = [];
  public IReadOnlyCollection<Client> Clients => _clients.AsReadOnly();
  private readonly List<Pet> _pets = [];
  public IReadOnlyCollection<Pet> Pets => _pets.AsReadOnly();

  public DatabaseSeeder()
  {
    //Set the randomizer seed if you wish to generate repeatable data sets.
    Randomizer.Seed = new Random(34845);
    Faker.DefaultStrictMode = true;

    _clients.AddRange(GenerateClients(amount: 250));

    int[] items = [1, 2, 3, 4, 5];
    var weights = new[] { 0.6f, 0.2f, 0.15f, 0.03f, 0.02f };

    _pets.AddRange(GeneratePetsForClients(items, weights));
  }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
  private static IReadOnlyCollection<Client> GenerateClients(int amount)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
  {
    var idIterator = 0;
    var clientFaker = new Faker<Client>()
      .RuleFor(c => c.Id, f => idIterator++)
      .RuleFor(c => c.FirstName, f => f.Person.FirstName)
      .RuleFor(c => c.LastName, f => f.Person.LastName)
      .RuleFor(c => c.PhoneCountryCode, "+1")
      .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber("###-##-####"))
      .RuleFor(c => c.PhoneExtension, f => f.Random.Replace("###").OrNull(f, .92f))
      .RuleFor(c => c.Status, f => ClientAccountStatus.New)
      .RuleFor(c => c.CreatedAt, f => CapturedDateTime.CaptureFromLocalBclDateTime(DateTime.Now, true, "America/New_York"))
      .RuleFor(c => c.EmailAddress, f => f.Person.Email);

    var clients = Enumerable.Range(1, amount)
      .Select(i => SeedRow(clientFaker, i))
      .ToList();

    return clients;
  }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
  private IReadOnlyCollection<Pet> GeneratePetsForClients(int[] items, float[] weights)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
  {
    List<Pet> pets = [];
    var random = new Bogus.Randomizer();

    var pf = new Faker("en");
    var oldestBirthDate = pf.Date.Past(20);
    var youngestBirthDate = _dateReference.AddDays(-1);

    PetStatus[] petStatuses = [PetStatus.New, PetStatus.Active, PetStatus.Inactive];
    var puppyPetStatusWeights = new[] { 0.7f, 0.28f, 0.02f };
    var adultStatusWeights = new[] { 0.1f, 0.8f, 0.1f };
    var seniorStatusWeights = new[] { 0.05f, 0.55f, 0.4f };

    var petIdIterator = 0;
    foreach (var client in Clients)
    {
      var countOfClientPets = random.WeightedRandom(items, weights);
      for (var p = 0; p < countOfClientPets - 1; p++)
      {
        var birthDate = DateOnly.FromDateTime(pf.Date.Between(oldestBirthDate, youngestBirthDate));
        int age = (int)Math.Floor((int)(_dateReference - birthDate.ToDateTime(TimeOnly.MinValue)).TotalDays / 365.25m);
        // Set CreatedAt relative to birth date (e.g. usually 2+ months after between range of 1 day after birth and 1.5 years after birth).
        var createdAt = CapturedDateTime.CaptureFromLocalBclDateTime(birthDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).AddDays(pf.Random.Int(1, 547)), true, "America/New_York");
        var status = age <= 1
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
          : DateOnly.FromDateTime(birthDate.ToDateTime(TimeOnly.MinValue).AddMonths(pf.Random.Int(1, (int)(age * 12))));
        DateOnly? mostRecentVisitDate = firstVisitDate is null
          ? null
          : DateOnly.FromDateTime(((DateOnly)firstVisitDate).ToDateTime(TimeOnly.MinValue).AddDays(pf.Random.Int(1, 60)));

        OptionalNewPetData data = new(weight, age, birthDate, adoptionDate, deathDate, firstVisitDate, mostRecentVisitDate);
        var pet = new Pet(client.Id, pf.Person.FirstName, AnimalType.Dog, 
          GetRandomBreed(pf), createdAt, data)
        {
          Id = petIdIterator++
        };
        pets.Add(pet);
      }
    }

    return pets;
  }

  private static string GetRandomBreed(Faker faker)
  {
    // TODO: Use a list of breeds: https://github.com/paiv/fci-breeds/blob/main/fci-breeds.csv
    var breeds = new[] { "Labrador Retriever", "German Shepherd", "Golden Retriever", "Bulldog", "Poodle", "Beagle", "Rottweiler", "Yorkshire Terrier", "Boxer", "Dachshund", "Siberian Husky", "Great Dane", "Doberman Pinscher", "Australian Shepherd", "Cavalier King Charles Spaniel", "Shih Tzu", "Boston Terrier", "Pembroke Welsh Corgi", "Maltese", "Chihuahua", "Pomeranian", "Shetland Sheepdog", "Miniature Schnauzer", "French Bulldog", "Cocker Spaniel", "Pug", "Weimaraner", "Border Collie", "Basset Hound", "Vizsla", "West Highland White Terrier", "Rhodesian Ridgeback", "Bernese Mountain Dog", "Havanese", "Newfoundland", "Bichon Frise", "Bloodhound", "Akita", "Papillon", "Belgian Malinois", "Alaskan Malamute", "Whippet", "Chinese Shar-Pei", "Samoyed", "Scottish Terrier", "Soft Coated Wheaten Terrier", "Portuguese Water Dog", "Bullmastiff", "Airedale Terrier", "Bull Terrier", "Irish Setter", "Cairn Terrier", "Italian Greyhound", "Collie", "Dalmatian", "German Shorthaired Pointer", "English Springer Spaniel", "Chow Chow", "Pekingese", "Lhasa Apso", "Borzoi", "Afghan Hound", "Old English Sheepdog", "Basenji", "Keeshond", "American Eskimo Dog", "Saluki", "Standard Schnauzer", "Irish Wolfhound", "Brussels Griffon", "Japanese Chin", "Norwegian Elkhound", "Silky Terrier", "Toy Fox Terrier", "Tibetan Terrier", "Wirehaired Pointing Griffon", "Black and Tan Coonhound", "Giant Schnauzer", "Gordon Setter", "English Toy Spaniel", "Norwich Terrier", "Pharaoh Hound", "Ibizan Hound"};
    return faker.PickRandom(breeds);
  }

  private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class
  {
    var recordRow = faker.UseSeed(rowId).Generate();
    return recordRow;
  }
}
