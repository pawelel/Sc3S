using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", x => x.IsTemporal());
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleId).ValueGeneratedOnAdd();
        builder.Property(r => r.Name).IsRequired();
        builder.HasData(new Role()
        {
            Name = "Admin",
            RoleId = "1320173d-7e65-44c2-82ca-973c3cf1bdf4"
       ,
            CreatedBy = "seed",
            CreatedOn = DateTime.Now,
            UpdatedBy = "seed",
            UpdatedOn = DateTime.Now
        }, new()
        {
            Name = "Manager",
            RoleId = "4de524ca-176d-44b3-aa26-15c17ba2ea0d"
        ,
            CreatedBy = "seed",
            CreatedOn = DateTime.Now,
            UpdatedBy = "seed",
            UpdatedOn = DateTime.Now
        },
        new()
        {
            Name = "User",
            RoleId = "19d9ba04-7570-4789-8720-8c4fd24fc272"
        ,
            CreatedBy = "seed",
            CreatedOn = DateTime.Now,
            UpdatedBy = "seed",
            UpdatedOn = DateTime.Now
        }

        );
    }
}