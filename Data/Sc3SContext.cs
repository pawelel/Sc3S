
using Microsoft.EntityFrameworkCore;

using Sc3S.Entities;

using System.Reflection;

namespace Sc3S.Data;

public class Sc3SContext : DbContext
{
    public Sc3SContext(DbContextOptions<Sc3SContext> options)
        : base(options)
    {
    }

    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
