using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3S.Entities;
namespace Sc3S.Data.Configurations;
public class CommunicateCategoryConfig : IEntityTypeConfiguration<CommunicateCategory>
{
    public void Configure(EntityTypeBuilder<CommunicateCategory> builder)
    {
        builder.ToTable("CommunicateCategories", x => x.IsTemporal());
        builder.HasKey(x => new { x.CommunicateId, x.CategoryId });
        builder.Property(x => x.CommunicateId).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.HasOne(x => x.Communicate).WithMany(x => x.CommunicateCategories).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category).WithMany(x => x.CommunicateCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
