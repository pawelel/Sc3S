using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class DetailConfig : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.ToTable("Details", x => x.IsTemporal());
        builder.HasKey(x => x.DetailId);
        builder.Property(x => x.DetailId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasData(
            new Detail { DetailId = 1, Name = "IP", Description = "Podstawowy adres IP" },
            new Detail { DetailId = 2, Name = "MAC", Description = "Adres MAC" },
            new Detail { DetailId = 3, Name = "Hostname", Description = "Nazwa hosta" },
            new Detail { DetailId = 4, Name = "OS", Description = "System operacyjny" },
            new Detail { DetailId = 5, Name = "CPU", Description = "Procesor" },
            new Detail { DetailId = 6, Name = "RAM", Description = "Pamięć RAM" },
            new Detail { DetailId = 7, Name = "Pamięć", Description = "Dysk twardy" },
            new Detail { DetailId = 8, Name = "Karta graficzna", Description = "Karta graficzna" },
            new Detail { DetailId = 9, Name = "Karta sieciowa", Description = "Karta sieciowa" },
            new Detail { DetailId = 10, Name = "Karta rozszerzeń", Description = "Karta rozszerzeń" },
            new Detail { DetailId = 11, Name = "Zasilacz", Description = "Zasilacz" }
            );
    }
}