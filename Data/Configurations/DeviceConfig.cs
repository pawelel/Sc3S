using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class DeviceConfig : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices", x => x.IsTemporal());
        builder.HasKey(x => x.DeviceId);
        builder.Property(x => x.DeviceId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasData(
            new Device { DeviceId = 1, Name = "drukarka", Description = "Urządzenie drukujące", CreatedBy = "App", CreatedOn = DateTime.Now, UpdatedBy = "admin", UpdatedOn = DateTime.Now },
            new Device { DeviceId = 2, Name = "komputer", Description = "Urządzenie komputerowe" },
            new Device { DeviceId = 3, Name = "monitor", Description = "Wyświetlacz" },
            new Device { DeviceId = 4, Name = "Skaner", Description = "Skaner kodów" }
            );
    }
}