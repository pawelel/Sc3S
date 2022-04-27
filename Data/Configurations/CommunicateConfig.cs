using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class CommunicateConfig : IEntityTypeConfiguration<Communicate>
{
    public void Configure(EntityTypeBuilder<Communicate> builder)
    {
        builder.ToTable("Communicates", x => x.IsTemporal());
        builder.HasKey(x => x.CommunicateId);
        builder.Property(x => x.CommunicateId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Scope).IsRequired();
    }
}