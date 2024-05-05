using System.Reflection;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.PetAggregate;
using DaySpaPet.WebApi.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DaySpaPet.WebApi.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
  private readonly IDomainEventDispatcher? _dispatcher;

  public AppDbContext(DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher)
      : base(options)
  {
    _dispatcher = dispatcher;
  }

  public DbSet<Client> Clients => Set<Client>();
  public DbSet<Pet> Pets => Set<Pet>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

#if DEBUG
    SetupSeedData(modelBuilder);
#endif
  }

  private static void SetupSeedData(ModelBuilder modelBuilder)
  {
    // Generate seed data with Bogus, which will then be used in migrations
    var dbSeeder = new DatabaseSeeder();

    // Apply the seed data on the tables
    modelBuilder.Entity<Client>().HasData(dbSeeder.Clients);
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    var before = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    Console.WriteLine($"Emitting ({entitiesWithEvents.Count()}) domain events");
    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}
