﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace EntityPractica_otravez_;

public partial class WorldContext : DbContext
{
    public WorldContext()
    {
    }

    public WorldContext(DbContextOptions<WorldContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Countrylanguage> Countrylanguages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=world;uid=root;pwd=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.42-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("city");

            entity.HasIndex(e => e.CountryCode, "CountryCode");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.District)
                .HasMaxLength(20)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(35)
                .HasDefaultValueSql("''")
                .IsFixedLength();

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("city_ibfk_1");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PRIMARY");

            entity.ToTable("country");

            entity.Property(e => e.Code)
                .HasMaxLength(3)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.Code2)
                .HasMaxLength(2)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.Continent)
                .HasDefaultValueSql("'Asia'")
                .HasColumnType("enum('Asia','Europe','North America','Africa','Oceania','Antarctica','South America')");
            entity.Property(e => e.Gnp)
                .HasPrecision(10, 2)
                .HasColumnName("GNP");
            entity.Property(e => e.Gnpold)
                .HasPrecision(10, 2)
                .HasColumnName("GNPOld");
            entity.Property(e => e.GovernmentForm)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.HeadOfState)
                .HasMaxLength(60)
                .IsFixedLength();
            entity.Property(e => e.LifeExpectancy).HasPrecision(3, 1);
            entity.Property(e => e.LocalName)
                .HasMaxLength(45)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(52)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.Region)
                .HasMaxLength(26)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.SurfaceArea).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Countrylanguage>(entity =>
        {
            entity.HasKey(e => new { e.CountryCode, e.Language })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("countrylanguage");

            entity.HasIndex(e => e.CountryCode, "CountryCode");

            entity.Property(e => e.CountryCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.Language)
                .HasMaxLength(30)
                .HasDefaultValueSql("''")
                .IsFixedLength();
            entity.Property(e => e.IsOfficial)
                .HasDefaultValueSql("'F'")
                .HasColumnType("enum('T','F')");
            entity.Property(e => e.Percentage).HasPrecision(4, 1);

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Countrylanguages)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("countryLanguage_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
