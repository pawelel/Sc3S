using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class AccountConfig : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", x => x.IsTemporal());
        builder.HasOne(r => r.Role).WithMany(x => x.Accounts).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasKey(a => a.UserId);
        builder.Property(a => a.UserId).ValueGeneratedOnAdd();
        builder.Property(a => a.UserName).IsRequired();
        builder.Property(a => a.Email).IsRequired();
    }
}