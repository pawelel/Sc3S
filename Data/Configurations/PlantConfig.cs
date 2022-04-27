using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class PlantConfig : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.ToTable("Plants", x => x.IsTemporal());
        builder.HasKey(x => x.PlantId);
        builder.Property(x => x.PlantId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);

        builder.HasData(

            new()
            {
                PlantId = 1,
                Name = "P35",
                Description = "Zakład Poznań Antoninek"
            },
            new()
            {
                PlantId = 2,
                Name = "P69",
                Description = "Zakład Crafter Września"
            });
    }
}