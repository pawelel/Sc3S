using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class ParameterConfig : IEntityTypeConfiguration<Parameter>
{
    public void Configure(EntityTypeBuilder<Parameter> builder)
    {
        builder.ToTable("Parameters", x => x.IsTemporal());
        builder.HasKey(x => x.ParameterId);
        builder.Property(x => x.ParameterId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(200);

        builder.HasData(
            new Parameter { ParameterId = 1, Name = "Wysokość", Description = "Wymiar od podłoża pionowo do góry", CreatedBy = "seed", CreatedOn = DateTime.Now, UpdatedBy = "seed", UpdatedOn = DateTime.Now },
            new Parameter { ParameterId = 2, Name = "Szerokość", Description = "Wymiar w najszerszym miejscu od lewej do prawej", CreatedBy = "seed", CreatedOn = DateTime.Now, UpdatedBy = "seed", UpdatedOn = DateTime.Now },
            new Parameter { ParameterId = 3, Name = "Długość", Description = "Wymiar od najbardziej wysuniętego elementu z przodu urządzenia do tyłu", CreatedBy = "seed", CreatedOn = DateTime.Now, UpdatedBy = "seed", UpdatedOn = DateTime.Now },
            new Parameter { ParameterId = 4, Name = "Producent", Description = "Nazwa producenta", CreatedBy = "seed", CreatedOn = DateTime.Now, UpdatedBy = "seed", UpdatedOn = DateTime.Now },
            new Parameter { ParameterId = 5, Name = "Rozdzielczość", Description = "Rozdzielczość ekranu", CreatedBy = "seed", CreatedOn = DateTime.Now, UpdatedBy = "seed", UpdatedOn = DateTime.Now }
            );
    }
}