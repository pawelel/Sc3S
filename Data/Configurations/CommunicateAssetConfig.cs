using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class CommunicateAssetConfig : IEntityTypeConfiguration<CommunicateAsset>
{
    public void Configure(EntityTypeBuilder<CommunicateAsset> builder)
    {
        builder.ToTable("CommunicateAssets", x => x.IsTemporal());
        builder.HasKey(x => new { x.CommunicateId, x.AssetId });
        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.CommunicateId).IsRequired();
        builder.HasOne(x => x.Asset).WithMany(x => x.CommunicateAssets).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateAssets).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
    }
}