using DaySpaPet.WebApi.Core.AppUserAggregate;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Core.PetAggregate;
using DaySpaPet.WebApi.SharedKernel;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DaySpaPet.WebApi.Infrastructure.Data;

public partial class AppDbContext : DbContext {
  private readonly IDomainEventDispatcher? _dispatcher;

  public AppDbContext(DbContextOptions<AppDbContext> options,
          IDomainEventDispatcher? dispatcher)
                  : base(options) {
    _dispatcher = dispatcher;
  }

  public DbSet<AppUser> AppUsers => Set<AppUser>();
  public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();
  public DbSet<AppUserAssignedRole> AppUserAssignedRoles => Set<AppUserAssignedRole>();
  public DbSet<AppUserRefreshToken> AppUserRefreshTokens => Set<AppUserRefreshToken>();
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<Pet> Pets => Set<Pet>();

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

#if DEBUG
    // Generate seed data with Bogus, which will then be used in migrations
    BB.BubbleBuddiesDatabaseSeeder dbSeeder = new();

    // Apply the seed data on the tables
    modelBuilder.Entity<Client>().HasData(dbSeeder.Clients);
    modelBuilder.Entity<Pet>().HasData(dbSeeder.Pets);
#endif
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseExceptionProcessor();
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
    EntityBase[] before = ChangeTracker.Entries<EntityBase>()
                    .Select(e => e.Entity)
                    .Where(e => e.DomainEvents.Any())
                    .ToArray();
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null)
      return result;

    // dispatch events only if save was successful
    EntityBase[] entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
                    .Select(e => e.Entity)
                    .Where(e => e.DomainEvents.Any())
                    .ToArray();

    Console.WriteLine($"Emitting ({entitiesWithEvents.Length}) domain events");
    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges() {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}