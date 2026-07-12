using Microsoft.EntityFrameworkCore;

namespace Travellers.Support.Db;

public class TravellersDbContext(DbContextOptions<TravellersDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TravellersDbContext).Assembly);
    }
}
