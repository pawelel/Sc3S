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

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property("CreatedAt").CurrentValue = now;
                    break;

                case EntityState.Modified:
                    entry.Property("UpdatedAt").CurrentValue = now;
                    break;
            }
        }
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Role> Roles => Set<Role>();

    //stuff
    public virtual DbSet<Asset> Assets => Set<Asset>();

    public virtual DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();

    internal Task FirstOrDefaultAsync()
    {
        throw new NotImplementedException();
    }

    public virtual DbSet<AssetDetail> AssetDetails => Set<AssetDetail>();
    public virtual DbSet<Category> Categories => Set<Category>();
    public virtual DbSet<Detail> Details => Set<Detail>();
    public virtual DbSet<Device> Devices => Set<Device>();
    public virtual DbSet<Model> Models => Set<Model>();
    public virtual DbSet<ModelParameter> ModelParameters => Set<ModelParameter>();
    public virtual DbSet<Parameter> Parameters => Set<Parameter>();
    public virtual DbSet<Plant> Plants => Set<Plant>();

    // information
    public virtual DbSet<CommunicateArea> CommunicateAreas => Set<CommunicateArea>();

    public virtual DbSet<CommunicateAsset> CommunicateAssets => Set<CommunicateAsset>();
    public virtual DbSet<Communicate> Communicates => Set<Communicate>();
    public virtual DbSet<CommunicateCoordinate> CommunicateCoordinates => Set<CommunicateCoordinate>();
    public virtual DbSet<CommunicateDevice> CommunicateDevices => Set<CommunicateDevice>();
    public virtual DbSet<CommunicateModel> CommunicateModels => Set<CommunicateModel>();
    public virtual DbSet<CommunicateSpace> CommunicateSpaces => Set<CommunicateSpace>();
    public virtual DbSet<CommunicateCategory> CommunicateCategories => Set<CommunicateCategory>();

    // location
    public virtual DbSet<Area> Areas => Set<Area>();

    public virtual DbSet<Coordinate> Coordinates => Set<Coordinate>();
    public virtual DbSet<Space> Spaces => Set<Space>();

    // occurence
    public virtual DbSet<DeviceSituation> DeviceSituations => Set<DeviceSituation>();

    public virtual DbSet<CategorySituation> CategorySituations => Set<CategorySituation>();
    public virtual DbSet<Question> Questions => Set<Question>();
    public virtual DbSet<Situation> Situations => Set<Situation>();
    public virtual DbSet<SituationQuestion> SituationQuestions => Set<SituationQuestion>();
    public virtual DbSet<SituationDetail> SituationDetails => Set<SituationDetail>();
    public virtual DbSet<SituationParameter> SituationParameters => Set<SituationParameter>();
    public virtual DbSet<AssetSituation> AssetSituations => Set<AssetSituation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}