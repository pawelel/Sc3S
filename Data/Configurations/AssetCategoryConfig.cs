using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3S.Entities;
namespace Sc3S.Data.Configurations;
public class AssetCategoryConfig : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("AssetCategories", x=>x.IsTemporal());
        builder.HasKey(x => new { x.AssetId, x.CategoryId });
        builder.Property(x=>x.AssetId).IsRequired();
        builder.Property(x=>x.CategoryId).IsRequired();
        builder.HasOne(x => x.Asset).WithMany(x => x.AssetCategories).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category).WithMany(x => x.AssetCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
