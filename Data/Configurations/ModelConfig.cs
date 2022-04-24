﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3S.Entities;
namespace Sc3S.Data.Configurations;
public class ModelConfig : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("Models", x => x.IsTemporal());
        builder.HasKey(x => x.ModelId);
        builder.Property(x => x.ModelId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.DeviceId).IsRequired();
        builder.HasOne(x=>x.Device).WithMany(x=>x.Models).HasForeignKey(x=>x.DeviceId).OnDelete(DeleteBehavior.Restrict);
    }
}
