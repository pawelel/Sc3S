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

        IPasswordHasher<Account> hasher = new PasswordHasher<Account>();

        builder.HasData(
            new
            {
                UserId = "a8598d2a-9734-4544-b87f-d7d69aa790e9",
                UserName = "admin",
                Email = "admin@admin.com",
                RoleId = "1320173d-7e65-44c2-82ca-973c3cf1bdf4",
                CreatedBy = "seed",
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = "seed",
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false,
                PasswordHash = hasher.HashPassword(null!, "Maslo123$")
            },

      new
      {
          UserId = "1a95740d-b4fe-4ebf-965b-668fa67ea7cf",
          UserName = "user",
          Email = "user@user.com",
          RoleId = "19d9ba04-7570-4789-8720-8c4fd24fc272",
          CreatedBy = "seed",
          CreatedOn = DateTime.UtcNow,
          UpdatedBy = "seed",
          UpdatedOn = DateTime.UtcNow,
          IsDeleted = false,
          PasswordHash = hasher.HashPassword(null!, "Maslo123$")
      }
        );
    }
}