﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", x => x.IsTemporal());
        builder.HasKey(x => x.AssetId);
        builder.Property(x => x.AssetId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CoordinateId).IsRequired();
        builder.Property(x => x.ModelId).IsRequired();
        builder.HasOne(x => x.Model).WithMany(x => x.Assets).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Coordinate).WithMany(x => x.Assets).HasForeignKey(x => x.CoordinateId).OnDelete(DeleteBehavior.Restrict);
    }
}