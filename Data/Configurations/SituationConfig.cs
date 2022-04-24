using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sc3S.Entities;
namespace Sc3S.Data.Configurations;
public class SituationConfig : IEntityTypeConfiguration<Situation>
{
    public void Configure(EntityTypeBuilder<Situation> builder)
    {
        builder.ToTable("Situations", x => x.IsTemporal());
        builder.HasKey(x => x.SituationId);
        builder.Property(x => x.SituationId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);
    }
}
