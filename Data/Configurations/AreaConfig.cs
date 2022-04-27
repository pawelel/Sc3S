using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class AreaConfig : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas", x => x.IsTemporal());
        builder.HasKey(x => x.AreaId);
        builder.Property(x => x.AreaId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder.HasOne(x => x.Plant).WithMany(x => x.Areas).HasForeignKey(x => x.PlantId).IsRequired().OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new
            {
                AreaId = 1,
                Name = "Spawalnia",
                PlantId = 1,
                CreatedAt = DateTime.Now,
                CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                Description = "",
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
            },
            new
            {
                AreaId = 2,
                Name = "Lakiernia",
                PlantId = 1,
                CreatedAt = DateTime.Now,
                CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                Description = "",
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
            },
            new
            {
                AreaId = 3,
                Name = "Montaż",
                PlantId = 1,
                CreatedAt = DateTime.Now,
                CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                Description = "",
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
            },
            new
            {
                AreaId = 4,
                Name = "Spawalnia",
                PlantId = 2,
                CreatedAt = DateTime.Now,
                CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                Description = "",
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
            },
            new
            {
                AreaId = 5,
                Name = "Lakiernia",
                PlantId = 2,
                CreatedAt = DateTime.Now,
                CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                Description = "",
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
            },
            new
            {
                AreaId = 6,
                Name = "Montaż",
                PlantId = 2,
                CreatedAt = DateTime.Now,
                CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                Description = "",
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
            }
            ); ;
    }
}