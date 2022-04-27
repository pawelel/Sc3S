using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sc3S.Entities;

namespace Sc3S.Data.Configurations;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", x => x.IsTemporal());
        builder.HasKey(x => x.CategoryId);
        builder.Property(x => x.CategoryId).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();
        builder.HasData(
           new Category { CategoryId = 1, Name = "3d Mapping", Description = "PH 3DMAPPING" },
new Category { CategoryId = 2, Name = "ALS", Description = "PH ALS" },
new Category { CategoryId = 3, Name = "Drukarka Atlas", Description = "PH ATLAS DRUCKER" },
new Category { CategoryId = 4, Name = "Dell PC", Description = "PH DELLPC" },
new Category { CategoryId = 5, Name = "Drukarka Epson", Description = "PH EPSON DRUCKER" },
new Category { CategoryId = 6, Name = "FFT", Description = "PH FFT" },
new Category { CategoryId = 7, Name = "Drukarka Fis", Description = "PH FIS DRUCKER" },
new Category { CategoryId = 8, Name = "EQS", Description = "PH FIS EQS" },
new Category { CategoryId = 9, Name = "FPG", Description = "PH FPG" },
new Category { CategoryId = 10, Name = "GBA NEC", Description = "PH GBA NEC" },
new Category { CategoryId = 11, Name = "GBA Siemens", Description = "PH GBA SIEMENS" },
new Category { CategoryId = 12, Name = "Gom", Description = "PH GOM" },
new Category { CategoryId = 13, Name = "HDT Fis", Description = "PH HDT FIS" },
new Category { CategoryId = 14, Name = "HDT Logistik", Description = "PH HDT LOGISTIK" },
new Category { CategoryId = 15, Name = "HDT Zebra", Description = "PH HDT ZEBRA" },
new Category { CategoryId = 16, Name = "Jungman", Description = "PH JUNGMAN" },
new Category { CategoryId = 17, Name = "Drukarka logistyczna", Description = "PH LOGISTIK DRUCKER" },
new Category { CategoryId = 18, Name = "MasterPC", Description = "PH MASTERPC" },
new Category { CategoryId = 19, Name = "MDI Host", Description = "PH MDIHOST" },
new Category { CategoryId = 20, Name = "MFT", Description = "PH MFT" },
new Category { CategoryId = 21, Name = "OPS", Description = "PH OPS" },
new Category { CategoryId = 22, Name = "PBL", Description = "PH PBL" },
new Category { CategoryId = 23, Name = "PC produkcyjny", Description = "PH PC INDUSTRY" },
new Category { CategoryId = 24, Name = "Pegasus", Description = "PH PEGASUS" },
new Category { CategoryId = 25, Name = "Phoenix", Description = "PH PHOENIX" },
new Category { CategoryId = 26, Name = "Qs Torque", Description = "PH QSTORQUE" },
new Category { CategoryId = 27, Name = "Scout", Description = "PH SCOUT" },
new Category { CategoryId = 28, Name = "SIEMENS 477D", Description = "PH SIEMENS477D" },
new Category { CategoryId = 29, Name = "SIEMENS 477D pro", Description = "PH SIEMENS477DPRO" },
new Category { CategoryId = 30, Name = "SIEMENS 677D", Description = "PH SIEMENS677D" },
new Category { CategoryId = 31, Name = "Smartwatch", Description = "PH SMARTWATCH" },
new Category { CategoryId = 32, Name = "Support", Description = "PH SUPPORT SERVICES" },
new Category { CategoryId = 33, Name = "Tablet Panasonic", Description = "PH TABLET PANASONIC" },
new Category { CategoryId = 34, Name = "Tablet Surface", Description = "PH TABLET SURFACE" },
new Category { CategoryId = 35, Name = "Typenschild", Description = "PH TYPENSCHILD" },
new Category { CategoryId = 36, Name = "VCI", Description = "PH VCI" },
new Category { CategoryId = 37, Name = "VMT", Description = "PH VMT" },
new Category { CategoryId = 38, Name = "WINDOWS Server", Description = "PH WINDOWS SERVER" },
new Category { CategoryId = 39, Name = "Zeiss", Description = "PH ZEISS" }
            );
    }
}