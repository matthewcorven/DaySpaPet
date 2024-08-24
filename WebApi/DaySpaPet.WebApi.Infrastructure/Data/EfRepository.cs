using Ardalis.Specification.EntityFrameworkCore;
using DaySpaPet.WebApi.SharedKernel;

namespace DaySpaPet.WebApi.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot {
    public EfRepository(AppDbContext dbContext) : base(dbContext) {
    }
}