<<<<<<< HEAD
﻿using Microsoft.EntityFrameworkCore;

using Sc3S.Entities;

using System.Reflection;
=======
﻿
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Sc3S.Entities;
using System.Data;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
>>>>>>> 9324f5ea5a31ced57ebbea41ef3ebf749d2db1e7

namespace Sc3S.Data;

public class Sc3SContext : IdentityDbContext<ApplicationUser>
{
<<<<<<< HEAD
=======

>>>>>>> 9324f5ea5a31ced57ebbea41ef3ebf749d2db1e7
    public Sc3SContext(DbContextOptions<Sc3SContext> options)
        : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
<<<<<<< HEAD
=======
        var userId = "App";
>>>>>>> 9324f5ea5a31ced57ebbea41ef3ebf749d2db1e7
        var now = DateTime.Now;
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property("CreatedAt").CurrentValue = now;
                    break;

                case EntityState.Modified:
                    entry.Property("UpdatedAt").CurrentValue = now;
                    break;
            }
        }
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

<<<<<<< HEAD
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Role> Roles => Set<Role>();
=======

>>>>>>> 9324f5ea5a31ced57ebbea41ef3ebf749d2db1e7

    //stuff
    public virtual DbSet<Asset> Assets => Set<Asset>();

    public virtual DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();

    internal Task FirstOrDefaultAsync()
    {
        throw new NotImplementedException();
    }

    public virtual DbSet<AssetDetail> AssetDetails => Set<AssetDetail>();
    public virtual DbSet<Category> Categories => Set<Category>();
    public virtual DbSet<Detail> Details => Set<Detail>();
    public virtual DbSet<Device> Devices => Set<Device>();
    public virtual DbSet<Model> Models => Set<Model>();
    public virtual DbSet<ModelParameter> ModelParameters => Set<ModelParameter>();
    public virtual DbSet<Parameter> Parameters => Set<Parameter>();
    public virtual DbSet<Plant> Plants => Set<Plant>();

    // information
    public virtual DbSet<CommunicateArea> CommunicateAreas => Set<CommunicateArea>();

    public virtual DbSet<CommunicateAsset> CommunicateAssets => Set<CommunicateAsset>();
    public virtual DbSet<Communicate> Communicates => Set<Communicate>();
    public virtual DbSet<CommunicateCoordinate> CommunicateCoordinates => Set<CommunicateCoordinate>();
    public virtual DbSet<CommunicateDevice> CommunicateDevices => Set<CommunicateDevice>();
    public virtual DbSet<CommunicateModel> CommunicateModels => Set<CommunicateModel>();
    public virtual DbSet<CommunicateSpace> CommunicateSpaces => Set<CommunicateSpace>();
    public virtual DbSet<CommunicateCategory> CommunicateCategories => Set<CommunicateCategory>();
    // location
    public virtual DbSet<Area> Areas => Set<Area>();

    public virtual DbSet<Coordinate> Coordinates => Set<Coordinate>();
    public virtual DbSet<Space> Spaces => Set<Space>();

    // occurence
    public virtual DbSet<DeviceSituation> DeviceSituations => Set<DeviceSituation>();

    public virtual DbSet<CategorySituation> CategorySituations => Set<CategorySituation>();
    public virtual DbSet<Question> Questions => Set<Question>();
    public virtual DbSet<Situation> Situations => Set<Situation>();
    public virtual DbSet<SituationQuestion> SituationQuestions => Set<SituationQuestion>();
    public virtual DbSet<SituationDetail> SituationDetails => Set<SituationDetail>();
    public virtual DbSet<SituationParameter> SituationParameters => Set<SituationParameter>();
    public virtual DbSet<AssetSituation> AssetSituations => Set<AssetSituation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<Plant>(b =>
        {
            b.ToTable("Plants", x => x.IsTemporal());
            b.HasKey(x => x.PlantId);
            b.Property(x => x.PlantId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(200);

            b.HasData(

                new()
                {
                    PlantId = 1,
                    Name = "P35",
                    Description = "Zakład Poznań Antoninek"
                },
                new()
                {
                    PlantId = 2,
                    Name = "P69",
                    Description = "Zakład Crafter Września"
                });
        });
        builder.Entity<Area>(b =>
        {
            b.ToTable("Areas", x => x.IsTemporal());
            b.HasKey(x => x.AreaId);
            b.Property(x => x.AreaId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).IsRequired();
            b.HasOne(x => x.Plant).WithMany(x => x.Areas).HasForeignKey(x => x.PlantId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            b.HasData(
                new
                {
                    AreaId = 1,
                    Name = "Spawalnia",
                    PlantId = 1,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                    Description = "",
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
                },
                new
                {
                    AreaId = 2,
                    Name = "Lakiernia",
                    PlantId = 1,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                    Description = "",
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
                },
                new
                {
                    AreaId = 3,
                    Name = "Montaż",
                    PlantId = 1,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                    Description = "",
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
                },
                new
                {
                    AreaId = 4,
                    Name = "Spawalnia",
                    PlantId = 2,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                    Description = "",
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
                },
                new
                {
                    AreaId = 5,
                    Name = "Lakiernia",
                    PlantId = 2,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                    Description = "",
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
                },
                new
                {
                    AreaId = 6,
                    Name = "Montaż",
                    PlantId = 2,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de",
                    Description = "",
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "cfd23736-9c80-4ec8-9b1a-1dfb9132a5de"
                }
                );
        });
        builder.Entity<Space>(b =>
        {
            b.ToTable("Spaces", x => x.IsTemporal());
            b.HasKey(x => x.SpaceId);
            b.Property(x => x.SpaceId).ValueGeneratedOnAdd();
            b.Property(x => x.AreaId).IsRequired();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(200);
            b.HasOne(x => x.Area).WithMany(x => x.Spaces).HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Coordinate>(b =>
        {
            b.ToTable("Coordinates", x => x.IsTemporal());
            b.HasKey(x => x.CoordinateId);
            b.Property(x => x.CoordinateId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.SpaceId).IsRequired();
            b.HasOne(x => x.Space).WithMany(x => x.Coordinates).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Device>(b =>
        {
            b.ToTable("Devices", x => x.IsTemporal());
            b.HasKey(x => x.DeviceId);
            b.Property(x => x.DeviceId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);

            b.HasData(
                new Device { DeviceId = 1, Name = "drukarka", Description = "Urządzenie drukujące" },
                new Device { DeviceId = 2, Name = "komputer", Description = "Urządzenie komputerowe" },
                new Device { DeviceId = 3, Name = "monitor", Description = "Wyświetlacz" },
                new Device { DeviceId = 4, Name = "Skaner", Description = "Skaner kodów" }
                );
        });
        builder.Entity<Question>(b =>
        {
            b.ToTable("Questions", x => x.IsTemporal());
            b.HasKey(x => x.QuestionId);
            b.Property(x => x.QuestionId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });
        builder.Entity<Situation>(b =>
        {
            b.ToTable("Situations", x => x.IsTemporal());
            b.HasKey(x => x.SituationId);
            b.Property(x => x.SituationId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(60).IsRequired();
            b.Property(x => x.Description).HasMaxLength(200);
        });
        builder.Entity<Model>(b =>
        {
            b.ToTable("Models", x => x.IsTemporal());
            b.HasKey(x => x.ModelId);
            b.Property(x => x.ModelId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(200);
            b.Property(x => x.DeviceId).IsRequired();
            b.HasOne(x => x.Device).WithMany(x => x.Models).HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<Parameter>(b =>
        {
            b.ToTable("Parameters", x => x.IsTemporal());
            b.HasKey(x => x.ParameterId);
            b.Property(x => x.ParameterId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(200);

            b.HasData(
                new Parameter { ParameterId = 1, Name = "Wysokość", Description = "Wymiar od podłoża pionowo do góry" },
                new Parameter { ParameterId = 2, Name = "Szerokość", Description = "Wymiar w najszerszym miejscu od lewej do prawej" },
                new Parameter { ParameterId = 3, Name = "Długość", Description = "Wymiar od najbardziej wysuniętego elementu z przodu urządzenia do tyłu" },
                new Parameter { ParameterId = 4, Name = "Producent", Description = "Nazwa producenta" },
                new Parameter { ParameterId = 5, Name = "Rozdzielczość", Description = "Rozdzielczość ekranu" }
                );
        });
        builder.Entity<Category>(b =>
        {
            b.ToTable("Categories", x => x.IsTemporal());
            b.HasKey(x => x.CategoryId);
            b.Property(x => x.CategoryId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).IsRequired();
            b.HasData(
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
        });
        builder.Entity<Detail>(b =>
        {
            b.ToTable("Details", x => x.IsTemporal());
            b.HasKey(x => x.DetailId);
            b.Property(x => x.DetailId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);
            b.HasData(
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
        });
        builder.Entity<Communicate>(b =>
        {
            b.ToTable("Communicates", x => x.IsTemporal());
            b.HasKey(x => x.CommunicateId);
            b.Property(x => x.CommunicateId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Scope).IsRequired();
        });
        builder.Entity<Asset>(b =>
        {
            b.ToTable("Assets", x => x.IsTemporal());
            b.HasKey(x => x.AssetId);
            b.Property(x => x.AssetId).ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.CoordinateId).IsRequired();
            b.Property(x => x.ModelId).IsRequired();
            b.HasOne(x => x.Model).WithMany(x => x.Assets).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Coordinate).WithMany(x => x.Assets).HasForeignKey(x => x.CoordinateId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<AssetCategory>(b =>
        {
            b.ToTable("AssetCategories", x => x.IsTemporal());
            b.HasKey(x => new { x.AssetId, x.CategoryId });
            b.Property(x => x.AssetId).IsRequired();
            b.Property(x => x.CategoryId).IsRequired();
            b.HasOne(x => x.Asset).WithMany(x => x.AssetCategories).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Category).WithMany(x => x.AssetCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<AssetDetail>(b =>
        {
            b.ToTable("AssetDetails", x => x.IsTemporal());
            b.HasKey(x => new { x.AssetId, x.DetailId });
            b.Property(x => x.AssetId).IsRequired();
            b.Property(x => x.DetailId).IsRequired();
            b.HasOne(x => x.Asset).WithMany(x => x.AssetDetails).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Detail).WithMany(x => x.AssetDetails).HasForeignKey(x => x.DetailId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<AssetSituation>(b =>
        {
            b.ToTable("AssetSituations", x => x.IsTemporal());
            b.HasKey(x => new { x.AssetId, x.SituationId });
            b.Property(x => x.AssetId).IsRequired();
            b.Property(x => x.SituationId).IsRequired();
            b.HasOne(x => x.Asset).WithMany(x => x.AssetSituations).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Situation).WithMany(x => x.AssetSituations).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CategorySituation>(b =>
        {
            b.ToTable("CategorySituations", x => x.IsTemporal());
            b.HasKey(x => new { x.CategoryId, x.SituationId });
            b.Property(x => x.SituationId).IsRequired();
            b.Property(x => x.CategoryId).IsRequired();
            b.HasOne(x => x.Category).WithMany(x => x.CategorySituations).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Situation).WithMany(x => x.CategorySituations).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateArea>(b =>
        {
            b.ToTable("CommunicateAreas", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.AreaId });
            b.Property(x => x.AreaId).IsRequired();
            b.Property(x => x.CommunicateId).IsRequired();
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateAreas).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Area).WithMany(x => x.CommunicateAreas).HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateAsset>(b =>
        {
            b.ToTable("CommunicateAssets", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.AssetId });
            b.Property(x => x.AssetId).IsRequired();
            b.Property(x => x.CommunicateId).IsRequired();
            b.HasOne(x => x.Asset).WithMany(x => x.CommunicateAssets).HasForeignKey(x => x.AssetId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateAssets).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateCategory>(b =>
        {
            b.ToTable("CommunicateCategories", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.CategoryId });
            b.Property(x => x.CommunicateId).IsRequired();
            b.Property(x => x.CategoryId).IsRequired();
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateCategories).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Category).WithMany(x => x.CommunicateCategories).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateCoordinate>(b =>
        {
            b.ToTable("CommunicateCoordinates", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.CoordinateId });
            b.Property(x => x.CoordinateId).IsRequired();
            b.Property(x => x.CommunicateId).IsRequired();
            b.HasOne(x => x.Coordinate).WithMany(x => x.CommunicateCoordinates).HasForeignKey(x => x.CoordinateId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateCoordinates).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateDevice>(b =>
        {
            b.ToTable("CommunicateDevices", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.DeviceId });
            b.Property(x => x.DeviceId).IsRequired();
            b.Property(x => x.CommunicateId).IsRequired();
            b.HasOne(x => x.Device).WithMany(x => x.CommunicateDevices).HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateDevices).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateModel>(b =>
        {
            b.ToTable("CommunicateModels", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.ModelId });
            b.Property(x => x.ModelId).IsRequired();
            b.Property(x => x.CommunicateId).IsRequired();
            b.HasOne(x => x.Model).WithMany(x => x.CommunicateModels).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateModels).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CommunicateSpace>(b =>
        {
            b.ToTable("CommunicateSpaces", x => x.IsTemporal());
            b.HasKey(x => new { x.CommunicateId, x.SpaceId });
            b.Property(x => x.SpaceId).IsRequired();
            b.Property(x => x.CommunicateId).IsRequired();
            b.HasOne(x => x.Communicate).WithMany(x => x.CommunicateSpaces).HasForeignKey(x => x.CommunicateId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Space).WithMany(x => x.CommunicateSpaces).HasForeignKey(x => x.SpaceId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<DeviceSituation>(b =>
        {
            b.ToTable("DeviceSituations", x => x.IsTemporal());
            b.HasKey(x => new { x.DeviceId, x.SituationId });
            b.Property(x => x.SituationId).IsRequired();
            b.Property(x => x.DeviceId).IsRequired();
            b.HasOne(x => x.Device).WithMany(x => x.DeviceSituations).HasForeignKey(x => x.DeviceId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Situation).WithMany(x => x.DeviceSituations).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<ModelParameter>(b =>
        {
            b.ToTable("ModelParameters", x => x.IsTemporal());
            b.HasKey(x => new { x.ModelId, x.ParameterId });
            b.Property(x => x.ModelId).IsRequired();
            b.Property(x => x.ParameterId).IsRequired();
            b.Property(x => x.Value).HasMaxLength(50);
            b.HasOne(x => x.Model).WithMany(x => x.ModelParameters).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Parameter).WithMany(x => x.ModelParameters).HasForeignKey(x => x.ParameterId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<SituationDetail>(b =>
        {
            b.ToTable("SituationDetails", x => x.IsTemporal());
            b.HasKey(x => new { x.SituationId, x.DetailId });
            b.Property(x => x.SituationId).IsRequired();
            b.Property(x => x.DetailId).IsRequired();
            b.HasOne(x => x.Situation).WithMany(x => x.SituationDetails).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Detail).WithMany(x => x.SituationDetails).HasForeignKey(x => x.DetailId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<SituationParameter>(b =>
        {
            b.ToTable("SituationParameters", x => x.IsTemporal());
            b.HasKey(x => new { x.SituationId, x.ParameterId });
            b.Property(x => x.SituationId).IsRequired();
            b.Property(x => x.ParameterId).IsRequired();
            b.HasOne(x => x.Situation).WithMany(x => x.SituationParameters).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Parameter).WithMany(x => x.SituationParameters).HasForeignKey(x => x.ParameterId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<SituationQuestion>(b =>
        {
            b.ToTable("SituationQuestions", x => x.IsTemporal());
            b.HasKey(x => new { x.SituationId, x.QuestionId });
            b.Property(x => x.SituationId).IsRequired();
            b.Property(x => x.QuestionId).IsRequired();
            b.HasOne(x => x.Question).WithMany(x => x.SituationQuestions).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(x => x.Situation).WithMany(x => x.SituationQuestions).HasForeignKey(x => x.SituationId).OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<IdentityRole>(role =>
        {
            role.HasData(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = "8c916fc5-5d08-4164-8594-7ac0e2b6e16a", ConcurrencyStamp = "83256a0f-8959-4eb8-a15e-e9c74c782841" });
            role.HasData(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER", Id = "af138749-2fc8-4bcf-8492-fadb9e0d5415", ConcurrencyStamp = "6d68df77-faee-4dab-bb84-4c445d4cc7a1" });
            role.HasData(new IdentityRole() { Name = "User", NormalizedName = "USER", Id = "9588cfdb-8071-49c0-82cf-c51f20d305d2", ConcurrencyStamp = "83e0991b-0ddb-4291-bfe6-f9217019fde5" });
        });
        builder.Entity<ApplicationUser>(user =>
        {//a hasher to hash the password before seeding the user to the db
            var hasher = new PasswordHasher<ApplicationUser>();
            user.HasData(new ApplicationUser
            {
                Id = "a96d7c75-47f4-409b-a4d1-03f93c105647", // primary key
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                PasswordHash = hasher.HashPassword(null!, "SuperUser123$"),
                EmailConfirmed = true,
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM"
            });
            user.HasData(new ApplicationUser
            {
                Id = "223eea6c-5cfc-4413-ba83-257db573452c", // primary key
                UserName = "manager@manager.com",
                NormalizedUserName = "MANAGER@MANAGER.COM",
                PasswordHash = hasher.HashPassword(null!, "SuperUser123$"),
                EmailConfirmed = true,
                Email = "manager@manager.com",
                NormalizedEmail = "MANAGER@MANAGER.COM"
            });
            user.HasData(new ApplicationUser
            {
                Id = "5877932b-ce30-45be-a63f-12e5e6e42ed3", // primary key
                UserName = "user@user.com",
                NormalizedUserName = "USER@USER.COM",
                PasswordHash = hasher.HashPassword(null!, "SuperUser123$"),
                EmailConfirmed = true,
                Email = "user@user.com",
                NormalizedEmail = "USER@USER.COM"
            });
        });
        builder.Entity<IdentityUserRole<string>>().HasData(
                 new IdentityUserRole<string>
                 {
                     RoleId = "8c916fc5-5d08-4164-8594-7ac0e2b6e16a",
                     UserId = "a96d7c75-47f4-409b-a4d1-03f93c105647"
                 });
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "af138749-2fc8-4bcf-8492-fadb9e0d5415",
                UserId = "223eea6c-5cfc-4413-ba83-257db573452c"
            });
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "9588cfdb-8071-49c0-82cf-c51f20d305d2",
                UserId = "5877932b-ce30-45be-a63f-12e5e6e42ed3"
            });
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "af138749-2fc8-4bcf-8492-fadb9e0d5415",
                UserId = "5877932b-ce30-45be-a63f-12e5e6e42ed3"
            });

    }
<<<<<<< HEAD
}
=======
    

}

>>>>>>> 9324f5ea5a31ced57ebbea41ef3ebf749d2db1e7
