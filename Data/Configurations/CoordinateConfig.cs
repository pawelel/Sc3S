using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class CoordinateConfig : IEntityTypeConfiguration<Coordinate>
{
    public void Configure(EntityTypeBuilder<Coordinate> builder)
    {
        builder.ToTable("Coordinates", x => x.IsTemporal());
        builder.HasKey(x => x.CoordinateId);
        builder.Property(x => x.CoordinateId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SpaceId).IsRequired();
        builder.HasOne(x => x.Space).WithMany(x => x.Coordinates).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.Restrict);
    }
}