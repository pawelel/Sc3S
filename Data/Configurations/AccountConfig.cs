using Microsoft.AspNetCore.Identity;
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
        Account account = new()
        {
            UserId = "a8598d2a-9734-4544-b87f-d7d69aa790e9",
            UserName = "admin",
            Email = "admin@admin.com",
            RoleId = "1320173d-7e65-44c2-82ca-973c3cf1bdf4",
            IsDeleted = false
        };
        IPasswordHasher<Account> hasher = new PasswordHasher<Account>();
        var hash = hasher.HashPassword(account, "Maslo123$");

        builder.HasData(new
        {
            UserId = "a8598d2a-9734-4544-b87f-d7d69aa790e9",
            UserName = "admin",
            Email = "admin@admin.com",
            RoleId = "1320173d-7e65-44c2-82ca-973c3cf1bdf4",
            PasswordHash = hash
        });
    }
}