﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class DeviceSituationConfig : IEntityTypeConfiguration<DeviceSituation>
{
    public void Configure(EntityTypeBuilder<DeviceSituation> builder)
    {
        builder.ToTable("DeviceSituations", x => x.IsTemporal());
        builder.HasKey(x => new { x.DeviceId, x.SituationId });
        builder.Property(x => x.SituationId).IsRequired();
        builder.Property(x => x.DeviceId).IsRequired();
        builder.HasOne(x => x.Device).WithMany(x => x.DeviceSituations).HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Situation).WithMany(x => x.DeviceSituations).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict).OnDelete(DeleteBehavior.Restrict);
    }
}